using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolTesting : MonoBehaviour
{
    IObjectPool<ArcingProjectile> m_Pool;

    [SerializeField] private ArcingProjectile ProjectilePrefab;
    private bool collectionChecks = false;
    private int maxPoolSize = 1000;

    private void Start()
    {
        m_Pool = new ObjectPool<ArcingProjectile>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, 10, maxPoolSize);
    }

    private void OnDestroyPoolObject(ArcingProjectile projectile)
    {
        throw new NotImplementedException();
    }

    private void OnReturnedToPool(ArcingProjectile projectile)
    {
        throw new NotImplementedException();
    }

    private void OnTakeFromPool(ArcingProjectile projectile)
    {
        
    }

    private ArcingProjectile CreatePooledItem()
    {
        var proj =
            Instantiate(ProjectilePrefab, transform.position, Quaternion.identity);
        return proj;
    }
}
