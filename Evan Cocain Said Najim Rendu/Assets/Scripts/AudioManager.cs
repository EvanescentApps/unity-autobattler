using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource SFXSource;
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private AudioClip selectClip;
    [SerializeField] private AudioClip attackClip;
    [SerializeField] private AudioClip hurtClip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
      
        try
        {
  SFXSource.volume = Mathf.Clamp01(0.08f);
        musicSource.volume = Mathf.Clamp(musicSource.volume, 0f, 0.5f); 
        musicSource.clip = musicClip;
        musicSource.Play();        }
        catch (System.Exception e)
        {
            Debug.LogError("Error: " + e.Message);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void PlayAttack()
    {
        try
        {
            SFXSource.PlayOneShot(attackClip);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error: " + e.Message);
        }
    
    }

    public void PlaySelect()
    {
         try
        {
                SFXSource.PlayOneShot(selectClip, 0.1f);        }
        catch (System.Exception e)
        {
            Debug.LogError("Error: " + e.Message);
        }
       
    }

    public void PlayHurt()
    {
         try
        {
                      SFXSource.PlayOneShot(hurtClip, 0.2f);
      }
        catch (System.Exception e)
        {
            Debug.LogError("Error: " + e.Message);
        }
    }
}
