using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurretController : MonoBehaviour {

    [SerializeField] private float fireRate;
    [SerializeField] private float range;
    [SerializeField] private float bulletForce;
    
    [Space, SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    
    [Space, SerializeField] private GameObject axis1Pivot;
    [SerializeField] private string axis1Name;
    
    [Space, SerializeField] private GameObject axis2Pivot;
    [SerializeField] private string axis2Name;

    private float _time;
    
    void Update()
    {
        _time += Time.deltaTime;

        if (_time < fireRate) return;

        GameObject nearestEnemy = GetNearestEnemy();
        if (!nearestEnemy) return;

        Transform enemyVisual = nearestEnemy.transform.Find("Visual");
        if (axis1Pivot) RotatePivot(axis1Pivot.transform, "y", axis1Name, enemyVisual);
        if (axis2Pivot) RotatePivot(axis2Pivot.transform, "x", axis2Name, enemyVisual);
        
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * bulletForce);
        
        Destroy(bullet, 2);

        _time = 0;
    }

    private GameObject GetNearestEnemy() {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range);
        
        List<GameObject> enemiesInRange = hitColliders
            .Where(e => e.CompareTag("Enemy"))
            .Select(e => e.gameObject)
            .ToList();

        GameObject nearestEnemy = enemiesInRange
            .OrderBy(e => Vector3.Distance(transform.position, e.transform.position))
            .FirstOrDefault();

        return nearestEnemy;
    }

    private void RotatePivot(Transform pivot, string axisNameFrom, string axisNameTo, Transform pointAt) {
        Vector3 direction = (pointAt.position - bulletSpawnPoint.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);
        
        float axisFrom = 0;

        switch (axisNameFrom) {
            case "y": axisFrom = rotation.eulerAngles.y; break;
            case "x": axisFrom = rotation.eulerAngles.x; break;
        }

        switch (axisNameTo) {
            case "x": pivot.eulerAngles = new(axisFrom, pivot.eulerAngles.y, pivot.eulerAngles.z); break;
            case "y": pivot.eulerAngles = new(pivot.eulerAngles.x, axisFrom, pivot.eulerAngles.z); break;
            case "z": pivot.eulerAngles = new(pivot.eulerAngles.x, pivot.eulerAngles.y, axisFrom); break;
        }
    }

}
