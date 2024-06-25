using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    public GameObject[] WeaponImagePrefabs;

    private int ActiveWeaponIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        // GameObject a = Instantiate(WeaponImagePrefab, Vector3.zero, Quaternion.identity);
        // Debug.Log(a);
        // a.transform.SetParent(transform);
        // a.GetComponent<RectTransform>().localPosition = Vector3.zero;
    }

    public void AddNewWeaponImage(int index)
    {
        GameObject imageObject = Instantiate(WeaponImagePrefabs[index], Vector3.zero, Quaternion.identity);
        imageObject.transform.SetParent(transform);
        imageObject.GetComponent<RectTransform>().localPosition = Vector3.zero;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform image = transform.GetChild(i);
            int posY = 64 * (transform.childCount - i);
            image.localPosition = new Vector3(0, posY, 0);
        }
    }

    public void SetActiveWeaponImage(int index)
    {
        if (index >= transform.childCount)
        {
            return;
        }
        Image lastWeaponImage = transform.GetChild(ActiveWeaponIndex).GetComponent<Image>();
        lastWeaponImage.color = Color.black;

        Image currentWeaponImage = transform.GetChild(index).GetComponent<Image>();
        currentWeaponImage.color = Color.white;;
        ActiveWeaponIndex = index;

    }
}
