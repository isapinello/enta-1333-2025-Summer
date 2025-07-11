using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AudioSourcePrefabByType
{
    public SoundType type;
    public AudioSource prefab;
}
public class AudioPool : MonoBehaviour
{
    public List<AudioSourcePrefabByType> audioPrefabs;
    private Dictionary<SoundType, Queue<AudioSource>> poolMap = new();

    public void PlaySound(AudioClip clip, Vector3 pos, SoundType type)
    {
        if (!poolMap.ContainsKey(type))
            poolMap[type] = new Queue<AudioSource>();

        AudioSource source;

        if (poolMap[type].Count > 0)
        {
            source = poolMap[type].Dequeue();
        }
        else
        {
            var prefab = audioPrefabs.Find(p => p.type == type)?.prefab;
            if (prefab == null)
            {
                Debug.LogWarning("Missing prefab for sound type: " + type);
                return;
            }

            source = Instantiate(prefab, transform);
        }

        source.transform.position = pos;
        source.clip = clip;
        source.Play();
        StartCoroutine(ReleaseAfterPlay(source, type));
    }

    private IEnumerator ReleaseAfterPlay(AudioSource source, SoundType type)
    {
        yield return new WaitForSeconds(source.clip.length);
        poolMap[type].Enqueue(source);
    }
}
