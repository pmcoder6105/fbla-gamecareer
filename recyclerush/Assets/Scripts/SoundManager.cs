using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioClip[] cans;
    [SerializeField] AudioClip[] glass;
    [SerializeField] AudioClip[] compost;
    [SerializeField] AudioClip[] landfill;

    [SerializeField] AudioSource aSTrash;
    [SerializeField] AudioSource aSPoint;

    [SerializeField] AudioClip[] pointSounds;

    // Start is called before the first frame update
    void Start()
    {
        //aS = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayTrashSound(string name) {
        if (name.Contains("Can")) {
            int variant = Random.Range(0, cans.Length);
            aSTrash.PlayOneShot(cans[variant]);
        }
        else if (name.Contains("Bottle")) {
            int variant = Random.Range(0, glass.Length);
            aSTrash.PlayOneShot(glass[variant]);
        }
        else if (name.Contains("Core") || name.Contains("Bone")) {
            int variant = Random.Range(0, compost.Length);
            aSTrash.PlayOneShot(compost[variant]);
        }
        else if (name.Contains("Mug") || name.Contains("Broken")) {
            int variant = Random.Range(0, landfill.Length);
            aSTrash.PlayOneShot(landfill[variant]);
        }
    }

    public void PlayPointSound(bool correct) {
        if (correct)
            aSPoint.PlayOneShot(pointSounds[0]);
        else aSPoint.PlayOneShot(pointSounds[1]);
    }

}
