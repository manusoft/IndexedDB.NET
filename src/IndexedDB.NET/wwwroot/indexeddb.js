// =====================================================
// ManuHub.IndexedDB - Production JS Engine (FIXED)
// =====================================================

const databases = new Map();
const transactions = new Map();
const dbConfigs = new Map();

// -----------------------------------------------------
// SAFE DB VALIDATION
// -----------------------------------------------------

function isDbInvalid(db) {
    try {
        return !db || db.objectStoreNames === undefined;
    } catch {
        return true;
    }
}

// -----------------------------------------------------
// OPEN DATABASE (FIXED - NO STALE CONNECTIONS)
// -----------------------------------------------------

async function openDatabase(name, version, stores = []) {

    const key = `${name}_${version}`;

    dbConfigs.set(name, { version, stores });

    const cached = databases.get(key);

    if (cached) {

        if (isDbInvalid(cached)) {
            databases.delete(key);
        } else {
            return cached;
        }
    }

    const db = await new Promise((resolve, reject) => {

        const request = indexedDB.open(name, version);

        request.onupgradeneeded = (event) => {

            const db = event.target.result;

            for (const store of stores || []) {

                if (!db.objectStoreNames.contains(store.name)) {

                    const objectStore = db.createObjectStore(
                        store.name,
                        {
                            keyPath: store.keyPath,
                            autoIncrement: store.autoIncrement
                        }
                    );

                    for (const index of (store.indexes || [])) {
                        objectStore.createIndex(
                            index.name,
                            index.keyPath,
                            { unique: !!index.unique }
                        );
                    }
                }
            }
        };

        request.onsuccess = () => resolve(request.result);

        request.onerror = () => reject(request.error);

        request.onblocked = () => reject(new Error("IndexedDB blocked"));
    });

    // 🔥 handle upgrade safety
    db.onversionchange = () => {
        db.close();
        databases.delete(key);
    };

    databases.set(key, db);

    return db;
}

// -----------------------------------------------------
// CONFIG
// -----------------------------------------------------

function getConfig(name) {
    return dbConfigs.get(name) || {};
}

// -----------------------------------------------------
// INIT
// -----------------------------------------------------

export async function initializeDatabase(name, version, stores) {
    await openDatabase(name, version, stores);
}

export async function applyMigration(name, version, stores) {
    await openDatabase(name, version, stores);
    return true;
}

// -----------------------------------------------------
// TRANSACTIONS
// -----------------------------------------------------

export async function beginTransaction(name, stores, mode) {

    const cfg = getConfig(name);
    const db = await openDatabase(name, cfg.version, cfg.stores);

    const tx = db.transaction(stores, mode);
    const id = crypto.randomUUID();

    transactions.set(id, tx);

    tx.oncomplete = () => transactions.delete(id);
    tx.onerror = () => transactions.delete(id);
    tx.onabort = () => transactions.delete(id);

    return id;
}

export async function commitTransaction(id) {
    const tx = transactions.get(id);
    if (tx?.commit) tx.commit();
    transactions.delete(id);
}

export async function abortTransaction(id) {
    const tx = transactions.get(id);
    if (tx) tx.abort();
    transactions.delete(id);
}

// -----------------------------------------------------
// CORE EXECUTOR (SAFE)
// -----------------------------------------------------

async function execute(dbName, storeName, mode, action) {

    const cfg = getConfig(dbName);
    const db = await openDatabase(dbName, cfg.version, cfg.stores);

    if (!db.objectStoreNames.contains(storeName)) {
        throw new Error(`Store '${storeName}' not ready`);
    }

    return new Promise((resolve, reject) => {

        try {

            const tx = db.transaction(storeName, mode);
            const store = tx.objectStore(storeName);

            const request = action(store);

            request.onsuccess = () => resolve(request.result);

            request.onerror = () => {

                const err = request.error;

                if (err?.name === "ConstraintError") {
                    reject(new Error("DuplicateKeyException"));
                    return;
                }

                reject(err);
            };

        } catch (err) {
            reject(err);
        }
    });
}

// -----------------------------------------------------
// CRUD (FIXED)
// -----------------------------------------------------

export async function add(db, store, entity) {
    return execute(db, store, "readwrite", s => {

        if (!entity || typeof entity !== "object") {
            throw new Error("Invalid entity");
        }

        if (!entity.Id) {
            entity.Id = crypto.randomUUID();
        }

        return s.add(entity);
    });
}

export async function addRange(db, store, entities) {

    const cfg = getConfig(db);
    const database = await openDatabase(db, cfg.version, cfg.stores);

    return new Promise((resolve, reject) => {

        const tx = database.transaction(store, "readwrite");
        const s = tx.objectStore(store);

        for (const e of entities) {

            if (!e.Id) {
                e.Id = crypto.randomUUID();
            }

            s.add(e);
        }

        tx.oncomplete = () => resolve(true);
        tx.onerror = () => reject(tx.error);
    });
}

export async function put(db, store, entity) {

    return execute(db, store, "readwrite", s => {

        if (!entity || typeof entity !== "object") {
            throw new Error("Invalid entity");
        }

        if (!entity.Id) {
            entity.Id = crypto.randomUUID();
        }

        // 🔥 THIS IS THE KEY DIFFERENCE
        return s.put(entity);
    });
}

export async function remove(db, store, key) {
    return execute(db, store, "readwrite", s => s.delete(key));
}

export async function get(db, store, key) {
    return execute(db, store, "readonly", s => s.get(key));
}

export async function getAll(db, store) {

    const cfg = getConfig(db);
    const database = await openDatabase(db, cfg.version, cfg.stores);

    return new Promise((resolve, reject) => {

        const tx = database.transaction(store, "readonly");
        const s = tx.objectStore(store);

        const request = s.getAll();

        request.onsuccess = () => resolve(request.result);
        request.onerror = () => reject(request.error);
    });
}

// -----------------------------------------------------
// QUERY (FIXED SAFETY)
// -----------------------------------------------------

export async function queryWhereEquals(db, store, indexName, value) {

    const cfg = getConfig(db);
    const database = await openDatabase(db, cfg.version, cfg.stores);

    return new Promise((resolve, reject) => {

        const tx = database.transaction(store, "readonly");
        const s = tx.objectStore(store);

        let request;

        try {

            if (indexName && s.indexNames.contains(indexName)) {
                const index = s.index(indexName);
                request = index.getAll(value);
            }
            else {
                request = s.getAll();
            }

        } catch (err) {
            reject(new Error(`Invalid index '${indexName}'`));
            return;
        }

        request.onsuccess = () => {

            let result = request.result;

            if (!indexName && value !== undefined) {
                result = result.filter(x =>
                    Object.values(x).includes(value)
                );
            }

            resolve(result);
        };

        request.onerror = () => reject(request.error);
    });
}

// -----------------------------------------------------
// CURSOR
// -----------------------------------------------------

export async function cursorAll(db, store) {

    const cfg = getConfig(db);
    const database = await openDatabase(db, cfg.version, cfg.stores);

    return new Promise((resolve, reject) => {

        const tx = database.transaction(store, "readonly");
        const s = tx.objectStore(store);

        const results = [];
        const request = s.openCursor();

        request.onsuccess = (event) => {

            const cursor = event.target.result;

            if (cursor) {
                results.push(cursor.value);
                cursor.continue();
            } else {
                resolve(results);
            }
        };

        request.onerror = () => reject(request.error);
    });
}

// -----------------------------------------------------
// EXPORT DEBUG
// -----------------------------------------------------

export {
    databases,
    transactions,
    dbConfigs
};