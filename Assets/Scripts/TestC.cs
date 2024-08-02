using DG.Tweening; // Подключаем пространство имен DOTween
using UnityEngine;
public class Example : MonoBehaviour
{
    void Start()
    {
        // Анимируем перемещение объекта в позицию (5, 5, 5) за 1 секунду
        transform.DOMove(new Vector3(5, 5, 5), 1f);
    }
}
