using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using GameResource;
using CommonComponent;

namespace GameResource
{
    public static class ResourceManager
    {
        public static ulong PathToHash(string path)
        {
            return ResourceMainfest.GetPathHash(path);
        }

        public static string HashToPath(ulong pathHash)
        {
            return ResourceMainfest.GetHashPath(pathHash);
        }

        public static bool IsBundleExist(string path)
        {
            bool result = false;

            if (Application.isPlaying)
            {
                ulong hash = ResourceHash.FNVHashForPath(path);
                result = ResourceMainfest.IsAssetBundleResource(hash);
            }

            return result;
        }

        public static AssetBundle LoadAssetBundle(string path)
        {
            AssetBundle result = null;

            if (!string.IsNullOrEmpty(path) && Application.isPlaying)
            {
                ulong hash = ResourceHash.FNVHashForPath(path);
                result = AssetBundleManager.Instance.Load(hash);
            }

            return result;
        }

        public static void UnloadAssetBundle(string path)
        {
            if (!string.IsNullOrEmpty(path) && Application.isPlaying)
            {
                ulong hash = ResourceHash.FNVHashForPath(path);
                AssetBundleManager.Instance.Unload(hash);
            }
        }

        public static void ReloadAssetBundle()
        {
            if (Application.isPlaying)
            {
                AssetBundleManager.Instance.ReloadAssetBundle();
            }
        }

        // Load an asset stored at path in a Resources folder or AssetBundle
        public static T TryLoad<T>(string path) where T : Object
        {
            T result = default(T);

            if (!Application.isPlaying)
            {
                result = Resources.Load<T>(path);
            }
            else
            {
                ulong pathHash = PathToHash(path);
                result = TryLoad<T>(pathHash);
            }

            return result;
        }

        public static T TryLoad<T>(ulong pathHash) where T : Object
        {
            T result = default(T);

            if (!Application.isPlaying)
            {
                string path = HashToPath(pathHash);
                result = TryLoad<T>(path);
            }
            else
            {
                result = ResourceDataManager.Instance.Load(pathHash, typeof(T)) as T;
            }

            return result;
        }

        public static T Load<T>(string path) where T : Object
        {
            T result = default(T);

            result = TryLoad<T>(path);
            if (result == null)
            {
                Log.ErrorFormat("[ResourceManager]Failed to load({0}, {1})", path, typeof(T));
            }

            return result;
        }

        public static T Load<T>(ulong pathHash) where T : Object
        {
            T result = default(T);

            result = TryLoad<T>(pathHash);
            if (result == null)
            {
                Log.ErrorFormat("[ResourceManager]Failed to load({0}, {1})", pathHash, typeof(T));
            }

            return result;
        }

        public static ResourceLoadHandle LoadAsync<T>(string path, LoadingPriority priority = LoadingPriority.Normal) where T : Object
        {
            ResourceLoadHandle result = null;

            if (!Application.isPlaying)
            {
                result = new ResourceLoadHandle();
                result.progress = 1.0f;
                result.isDone = true;
                result.asset = Resources.Load(path, typeof(T));
            }
            else
            {
                ulong pathHash = PathToHash(path);
                result = LoadAsync<T>(pathHash, priority);
            }

            return result;
        }

        public static ResourceLoadHandle LoadAsync<T>(ulong pathHash, LoadingPriority priority = LoadingPriority.Normal) where T : Object
        {
            ResourceLoadHandle result = null;

            if (!Application.isPlaying)
            {
                string path = HashToPath(pathHash);
                result = LoadAsync<T>(path, priority);
            }
            else
            {
                result = ResourceDataManager.Instance.LoadAsync(pathHash, typeof(T), (int)priority);
            }

            return result;
        }

        public static void Unload(Object asset)
        {
            if (!Application.isPlaying)
            {
                if (!(asset as GameObject))
                {
                    Resources.UnloadAsset(asset);
                }
            }
            else
            {
                ResourceDataManager.Instance.Unload(asset);
            }
        }

        // spawn an GameObject stored at path in a Resources folder or AssetBundle
        public static GameObject Spawn(string path)
        {
            GameObject result = null;

            if (!Application.isPlaying)
            {
                Object asset = Load<GameObject>(path);
                if (asset != null)
                {
                    result = Object.Instantiate(asset) as GameObject;
                }
            }
            else
            {
                ulong pathHash = PathToHash(path);
                result = Spawn(pathHash);
            }

            return result;
        }

        public static GameObject Spawn(ulong pathHash)
        {
            GameObject result = null;

            if (!Application.isPlaying)
            {
                string path = HashToPath(pathHash);
                result = Spawn(path);
            }
            else
            {
                result = ResourcePoolManager.Instance.Spawn(pathHash);
            }

            return result;
        }

        public static ResourceSpawnHandle SpawnAsync(string path, LoadingPriority priority = LoadingPriority.Normal)
        {
            ResourceSpawnHandle result = null;

            if (!Application.isPlaying)
            {
                result = new ResourceSpawnHandle();
                result.progress = 1.0f;
                result.isDone = true;
                result.gameObject = Spawn(path);
            }
            else
            {
                ulong pathHash = PathToHash(path);
                result = SpawnAsync(pathHash, priority);
            }

            return result;
        }

        public static ResourceSpawnHandle SpawnAsync(ulong pathHash, LoadingPriority priority = LoadingPriority.Normal)
        {
            ResourceSpawnHandle result = null;

            if (!Application.isPlaying)
            {
                string path = HashToPath(pathHash);
                result = SpawnAsync(path, priority);
            }
            else
            {
                result = ResourcePoolManager.Instance.SpawnAsync(pathHash, (int)priority);
            }

            return result;
        }

        public static void Despawn(GameObject gameObject, DespawnType despawnType = DespawnType.Destroy)
        {
            if (!Application.isPlaying)
            {
                Object.DestroyImmediate(gameObject);
            }
            else
            {
                ResourcePoolManager.Instance.Despawn(gameObject, despawnType);
            }
        }

        public static void UnloadUnusedAssets()
        {
            if (Application.isPlaying)
            {
                ResourcePoolManager.Instance.UnloadUnusedPool();
                ResourceDataManager.Instance.UnloadUnusedAssets();
                List<ulong> pathList = ResourceDataManager.Instance.GetResourcePathList();
                AssetBundleManager.Instance.UnloadUnusedAssets(pathList);
                Resources.UnloadUnusedAssets();
            }
        }

        public static void ClearAssets()
        {
            if (Application.isPlaying)
            {
                ResourcePoolManager.Instance.ClearPool();
                ResourceDataManager.Instance.UnloadUnusedAssets();
                List<ulong> pathList = ResourceDataManager.Instance.GetResourcePathList();
                AssetBundleManager.Instance.UnloadUnusedAssets(pathList);
                Resources.UnloadUnusedAssets();
            }
        }
    }
}