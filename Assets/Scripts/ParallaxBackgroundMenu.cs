using UnityEngine;
using UnityEngine.Serialization;

public class ParallaxBackgroundMenu : MonoBehaviour
{
    private float startPos, length;
    [FormerlySerializedAs("camera")]
    public GameObject cameraCharacter;
    public float paralaxEffect;

    void Start() {
        startPos = transform.position.x;
        length   = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update() {
        float temp = cameraCharacter.transform.position.x * (1 - paralaxEffect);
        float dist = cameraCharacter.transform.position.x * paralaxEffect;

        // двигаем фон с поправкой на paralaxEffect
        transform.position = new Vector3(startPos + dist, transform.position.y, transform.position.z);

        // если камера перескочила спрайт, то меняем startPos
        if (temp > startPos + length)
            startPos += length;
        else if (temp < startPos - length)
            startPos -= length;

        cameraCharacter.transform.position = new Vector3(cameraCharacter.transform.position.x + 0.001f, cameraCharacter.transform.position.y, cameraCharacter.transform.position.z);
    }
}
