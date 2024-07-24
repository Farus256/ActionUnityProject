using UnityEngine;
using System.Collections.Generic;
public class ParallaxController : MonoBehaviour
{
    public GameObject backgroundPrefab;
    public float speed;
    private float imageWidth;
    private List<GameObject> backgrounds = new List<GameObject>();

    void Start()
    {
        imageWidth = backgroundPrefab.GetComponent<SpriteRenderer>().bounds.size.x;

        // Создаем два начальных фона
        //CreateBackground(new Vector3(0, 0, 0));
        CreateBackground(new Vector3(imageWidth, 0, 0));
    }

    void Update()
    {
        // Проверяем, нужно ли создать новый фон
        if (backgrounds.Count < 2)
        {
            CreateBackground(new Vector3(imageWidth, 0, 0));
        }
    }

    void CreateBackground(Vector3 position)
    {
        GameObject newBackground = Instantiate(backgroundPrefab, position, Quaternion.identity);

        backgrounds.Add(newBackground);
    }
}
