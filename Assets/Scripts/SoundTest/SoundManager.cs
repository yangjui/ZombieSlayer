using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] Sound[] sfx2D = null;
    [SerializeField] Sound[] sfx3D = null;
    [SerializeField] Sound[] bgm = null;
    [SerializeField] private AudioMixer mixer = null;

    [SerializeField] AudioSource bgmPlayer = null;
    [SerializeField] GameObject sfx2DPrefab = null;
    [SerializeField] GameObject sfx3DPrefab = null;
    private List<GameObject> sfx2DList = new List<GameObject>();
    private List<GameObject> sfx3DList = new List<GameObject>();
    private float timer;

    private void Start()
    {
        instance = this;
    }

    public void PlayBGM(string p_bgmName)
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            if (p_bgmName == bgm[i].name)
            {
                if (!bgmPlayer.isPlaying)
                {
                    bgmPlayer.clip = bgm[i].clip;
                    bgmPlayer.Play();
                }
            }
        }
    }

    public void StopBGM()
    {
        StartCoroutine(StopBGMCoroutine());
    }

    private IEnumerator StopBGMCoroutine()
    {
        float curVolume = bgmPlayer.volume;
        while (timer < 1.5f)
        {
            timer += Time.deltaTime;
            bgmPlayer.volume = Mathf.Lerp(0.1f, 0f, timer);
            yield return null;
        }
        bgmPlayer.Stop();
        bgmPlayer.volume = curVolume;
        timer = 0f;
    }

    public void Play2DSFX(string p_sfxName, Vector3 _position)
    {
        if (sfx2DList.Count == 0)
        {
            Add2DList();
        }
        
        for (int i = 0; i < sfx2D.Length; ++i)
        {
            if (p_sfxName == sfx2D[i].name)
            {
                for (int j = 0; j < sfx2DList.Count; j++)
                {
                    // SFXPlayer���� ��� ������ ���� Audio Source�� �߰��ߴٸ� 
                    if (!sfx2DList[j].GetComponent<AudioSource>().isPlaying)
                    {
                        sfx2DList[j].GetComponent<AudioSource>().clip = sfx2D[i].clip;
                        sfx2DList[j].GetComponent<AudioSource>().transform.position = _position;
                        sfx2DList[j].GetComponent<AudioSource>().Play();
                        return;
                    }
                }
                Add2DList();

                sfx2DList[i].GetComponent<AudioSource>().clip = sfx2D[i].clip;
                sfx2DList[i].GetComponent<AudioSource>().transform.position = _position;
                sfx2DList[i].GetComponent<AudioSource>().Play();
                return;
            }
        }
        Debug.Log(p_sfxName + " �̸��� ȿ������ �����ϴ�.");
        return;
    }

    private void Add2DList()
    {
        GameObject go = Instantiate(sfx2DPrefab, transform);
        sfx2DPrefab.GetComponent<AudioSource>().outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
        sfx2DList.Add(go);
    }

    public void Play3DSFX(string p_sfxName, Vector3 _position)
    {
        if (sfx3DList.Count == 0)
        {
            Add3DList();
        }

        for (int i = 0; i < sfx3D.Length; ++i)
        {
            if (p_sfxName == sfx3D[i].name)
            {
                for (int j = 0; j < sfx3DList.Count; j++)
                {
                    // SFXPlayer���� ��� ������ ���� Audio Source�� �߰��ߴٸ� 
                    if (!sfx3DList[j].GetComponent<AudioSource>().isPlaying)
                    {
                        sfx3DList[j].GetComponent<AudioSource>().clip = sfx3D[i].clip;
                        sfx3DList[j].GetComponent<AudioSource>().transform.position = _position;
                        sfx3DList[j].GetComponent<AudioSource>().Play();
                        return;
                    }
                }
                Add3DList();

                sfx3DList[i].GetComponent<AudioSource>().clip = sfx3D[i].clip;
                sfx3DList[i].GetComponent<AudioSource>().transform.position = _position;
                sfx3DList[i].GetComponent<AudioSource>().Play();
                return;
            }
        }
        Debug.Log(p_sfxName + " �̸��� ȿ������ �����ϴ�.");
        return;
    }

    private void Add3DList()
    {
        for (int i = 0; i < 5; ++i)
        {
            GameObject go = Instantiate(sfx3DPrefab, transform);
            sfx3DPrefab.GetComponent<AudioSource>().outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
            sfx3DList.Add(go);
        }
    }

    public void BGMVolume(float _val)
    {
        mixer.SetFloat("BGM", Mathf.Log10(_val) * 20);
    }

    public void SFXVolume(float _val)
    {
        mixer.SetFloat("SFX", Mathf.Log10(_val) * 20);
    }
}