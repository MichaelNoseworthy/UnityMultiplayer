using UnityEngine;
using UnityEngine.Networking;

public class AmmoSpawner : NetworkBehaviour {

    public GameObject ammoPrefab;
    public int numberOfAmmoObjects;

    private float minSpawnPosition = -28.0f;
    private float maxSpawnPosition = 28.0f;

    private float minSpawnRotation = 0.0f;
    private float maxSpawnRotation = 180.0f;

    public void spawnAmmo()
    {
        RpcspawnAmmo();

        for (int i = 0; i < numberOfAmmoObjects; i++)
        {
            var spawnPosition = new Vector3(
                Random.Range(minSpawnPosition, maxSpawnPosition),    
                2.0f,    
                Random.Range(minSpawnPosition, maxSpawnPosition));
            
            var spawnRotation = Quaternion.Euler(
                0.0f,
                Random.Range(minSpawnRotation, maxSpawnRotation),
                0.0f);

            var ammo = (GameObject)Instantiate(ammoPrefab, spawnPosition, spawnRotation);
            NetworkServer.Spawn(ammo);
        }
    }

    [ClientRpc]
    public void RpcspawnAmmo()
    {
        for (int i = 0; i < numberOfAmmoObjects; i++)
        {
            var spawnPosition = new Vector3(
                Random.Range(minSpawnPosition, maxSpawnPosition),
                0.0f,
                Random.Range(minSpawnPosition, maxSpawnPosition));

            var spawnRotation = Quaternion.Euler(
                0.0f,
                Random.Range(minSpawnRotation, maxSpawnRotation),
                0.0f);

            var ammo = (GameObject)Instantiate(ammoPrefab, spawnPosition, spawnRotation);
            NetworkServer.Spawn(ammo);
        }
    }

}
