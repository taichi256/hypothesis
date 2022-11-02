using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{
    private GameObject player;
    public GameObject ability;
    private GameObject mainCamera;
    private GameObject save;
    AbilityCount abilityCount;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("robo");
        mainCamera = GameObject.Find("Main Camera");
        abilityCount = ability.GetComponent<AbilityCount>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClickLoad()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main");
    }

    public void ClickNew()
    {
        PlayerPrefs.DeleteAll();
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main");
    }

    public void ClickSave()
    {
        StartCoroutine("Save");
    }

    public void ClickBack()
    {
        StartCoroutine("Save");
        SceneManager.LoadScene("Opening");
    }

    private IEnumerator Save()
    {
        PlayerPrefs.SetFloat("PLAYERPOSX", player.transform.position.x);
        PlayerPrefs.SetFloat("PLAYERPOSY", player.transform.position.y);
        PlayerPrefs.SetFloat("PLAYERPOSZ", player.transform.position.z);
        PlayerPrefs.SetFloat("CAMPOSX", mainCamera.transform.position.x);
        PlayerPrefs.SetFloat("CAMPOSY", mainCamera.transform.position.y);
        PlayerPrefs.SetFloat("CAMPOSZ", mainCamera.transform.position.z);
        PlayerPrefs.SetInt("AIRJUMP", abilityCount.AJLimit);
        PlayerPrefs.SetInt("DASH", abilityCount.DashLimit);
        PlayerPrefs.SetInt("GRAPPLE", abilityCount.GrappleLimit);
        save.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        save.gameObject.SetActive(false);
    }
}
