function openDatabase(databaseName, version, stores) {
    return new Promise((resolve, reject) => {
        const request = indexedDB.open(databaseName, version);

        request.onupgradeneeded = event => {
            const db = event.target.result;

            for (const store of stores) {
                if (!db.objectStoreNames.contains(store.name)) {
                    const objectStore = db.createObjectStore(store.name, {
                        keyPath: store.keyPath,
                        autoIncrement: store.autoIncrement
                    });

                    for (const index of store.indexes) {
                        objectStore.createIndex(
                            index.name,
                            index.keyPath,
                            {
                                unique: index.unique
                            });
                    }
                }
            }
        };

        request.onsuccess = () => resolve(request.result);

        request.onerror = () => reject(request.error);
    });
}


export async function initializeDatabase(
    databaseName,
    version,
    stores) {

    await openDatabase(databaseName, version, stores);
}

async function execute(databaseName, storeName, mode, action) {
    const db = await openDatabase(databaseName, 1, []);

    return new Promise((resolve, reject) => {
        const transaction = db.transaction(storeName, mode);

        const store = transaction.objectStore(storeName);

        const request = action(store);

        request.onsuccess = () => resolve(request.result);

        request.onerror = () => reject(request.error);
    });
}

export async function add(databaseName, storeName, entity) {
    return execute(databaseName, storeName, "readwrite", store => {
        return store.add(entity);
    });
}

export async function put(databaseName, storeName, entity) {
    return execute(databaseName, storeName, "readwrite", store => {
        return store.put(entity);
    });
}

export async function get(databaseName, storeName, key) {
    return execute(databaseName, storeName, "readonly", store => {
        return store.get(key);
    });
}

export async function getAll(databaseName, storeName) {
    return execute(databaseName, storeName, "readonly", store => {
        return store.getAll();
    });
}

export async function deleteRecord(databaseName, storeName, key) {
    return execute(databaseName, storeName, "readwrite", store => {
        return store.delete(key);
    });
}

export async function clear(databaseName, storeName) {
    return execute(databaseName, storeName, "readwrite", store => {
        return store.clear();
    });
}

export async function count(databaseName, storeName) {
    return execute(databaseName, storeName, "readonly", store => {
        return store.count();
    });
}