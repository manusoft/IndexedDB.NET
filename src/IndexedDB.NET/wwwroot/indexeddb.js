// =====================================================
// ManuHub.IndexedDB - Stable & Persistent Version
// =====================================================

const databases = new Map();
const transactions = new Map();
const dbConfigs = new Map();

// -----------------------------------------------------
// UTILITIES
// -----------------------------------------------------

function normalizeEntity(entity) {
    if (!entity || typeof entity !== "object") return entity;

    const normalized = { ...entity };

    // Critical: Ensure Id exists for keyPath
    if (normalized.Id === undefined && normalized.id === undefined) {
        normalized.Id = crypto.randomUUID();
    } else if (normalized.id !== undefined && normalized.Id === undefined) {
        normalized.Id = normalized.id;
        delete normalized.id;
    }

    return normalized;
}

// -----------------------------------------------------
// OPEN DATABASE (Most Stable)
// -----------------------------------------------------

async function openDatabase(name, version, stores = []) {
    const key = `${name}_${version}`;
    dbConfigs.set(name, { version, stores });

    const cached = databases.get(key);
    if (cached && !isDbInvalid(cached)) return cached;

    const db = await new Promise((resolve, reject) => {
        const request = indexedDB.open(name, version);

        request.onupgradeneeded = (event) => {
            const db = event.target.result;
            console.info(`[IndexedDB] Upgrading ${name} from v${event.oldVersion} to v${version}`);

            for (const store of stores || []) {
                if (!db.objectStoreNames.contains(store.name)) {
                    const objectStore = db.createObjectStore(store.name, {
                        keyPath: store.keyPath || "Id",
                        autoIncrement: !!store.autoIncrement
                    });

                    for (const index of store.indexes || []) {
                        if (!objectStore.indexNames.contains(index.name)) {
                            objectStore.createIndex(index.name, index.keyPath, {
                                unique: !!index.unique
                            });
                        }
                    }
                }
            }
        };

        request.onsuccess = () => resolve(request.result);
        request.onerror = () => reject(request.error);
        request.onblocked = () => reject(new Error("IndexedDB blocked"));
    });

    db.onversionchange = () => {
        db.close();
        databases.delete(key);
    };

    databases.set(key, db);
    return db;
}

function isDbInvalid(db) {
    try {
        return !db || typeof db.objectStoreNames === 'undefined';
    } catch {
        return true;
    }
}

export async function deleteDatabase(name) {
    return new Promise((resolve, reject) => {
        const request = indexedDB.deleteDatabase(name);
        request.onsuccess = () => {
            console.info(`[IndexedDB] Database '${name}' deleted`);
            databases.clear();
            dbConfigs.delete(name);
            resolve(true);
        };
        request.onerror = () => reject(request.error);
    });
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
// CRUD
// -----------------------------------------------------

async function execute(dbName, storeName, mode, action) {
    const cfg = dbConfigs.get(dbName) || {};
    const db = await openDatabase(dbName, cfg.version || 1, cfg.stores);

    if (!db.objectStoreNames.contains(storeName)) {
        throw new Error(`Store '${storeName}' not found`);
    }

    return new Promise((resolve, reject) => {
        const tx = db.transaction(storeName, mode);
        const store = tx.objectStore(storeName);
        const request = action(store);

        request.onsuccess = () => resolve(request.result);
        request.onerror = () => {
            const err = request.error;
            if (err?.name === "ConstraintError") {
                reject(new Error("DuplicateKeyException"));
            } else {
                reject(err);
            }
        };
    });
}

export async function add(db, store, entity) {
    return execute(db, store, "readwrite", s => s.add(normalizeEntity(entity)));
}

export async function addRange(db, store, entities) {
    const cfg = dbConfigs.get(db) || {};
    const database = await openDatabase(db, cfg.version || 1, cfg.stores);

    return new Promise((resolve, reject) => {
        const tx = database.transaction(store, "readwrite");
        const s = tx.objectStore(store);

        for (const e of entities) {
            s.add(normalizeEntity(e));
        }

        tx.oncomplete = () => resolve(true);
        tx.onerror = () => reject(tx.error);
    });
}

export async function put(db, store, entity) {
    return execute(db, store, "readwrite", s => s.put(normalizeEntity(entity)));
}

export async function putRange(db, store, entities) {
    const cfg = dbConfigs.get(db) || {};
    const database = await openDatabase(db, cfg.version || 1, cfg.stores);

    return new Promise((resolve, reject) => {
        const tx = database.transaction(store, "readwrite");
        const s = tx.objectStore(store);

        for (const e of entities) s.put(normalizeEntity(e));

        tx.oncomplete = () => resolve(true);
        tx.onerror = () => reject(tx.error);
    });
}

export async function remove(db, store, key) {
    return execute(db, store, "readwrite", s => s.delete(key));
}

export async function removeRange(db, store, keys) {
    const cfg = dbConfigs.get(db) || {};
    const database = await openDatabase(db, cfg.version || 1, cfg.stores);

    return new Promise((resolve, reject) => {
        const tx = database.transaction(store, "readwrite");
        const s = tx.objectStore(store);

        for (const key of keys) s.delete(key);

        tx.oncomplete = () => resolve(true);
        tx.onerror = () => reject(tx.error);
    });
}

export async function get(db, store, key) {
    return execute(db, store, "readonly", s => s.get(key));
}

export async function getAll(db, store) {
    const cfg = dbConfigs.get(db) || {};
    const database = await openDatabase(db, cfg.version || 1, cfg.stores);

    return new Promise((resolve, reject) => {
        const request = database.transaction(store, "readonly").objectStore(store).getAll();
        request.onsuccess = () => resolve(request.result);
        request.onerror = () => reject(request.error);
    });
}

export async function queryWhereEquals(db, store, indexName, value) {
    const cfg = dbConfigs.get(db) || {};
    const database = await openDatabase(db, cfg.version || 1, cfg.stores);

    return new Promise((resolve, reject) => {
        const tx = database.transaction(store, "readonly");
        const s = tx.objectStore(store);

        let request = indexName && s.indexNames.contains(indexName)
            ? s.index(indexName).getAll(value)
            : s.getAll();

        request.onsuccess = () => resolve(request.result);
        request.onerror = () => reject(request.error);
    });
}