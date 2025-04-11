using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Texture2D CursorTexture;

    private void Start()
    {
        // 设置鼠标光标
        Vector2 mousePos = Vector2.zero;
        UnityEngine.Cursor.SetCursor(CursorTexture, mousePos, CursorMode.Auto);
    }

    // 打乱卡片
    public void ShuffleCards()
    {
        // 获取所有卡片对象
        GameObject[] cards = GameObject.FindGameObjectsWithTag("Card");
        foreach (GameObject card in cards)
        {
            card.SendMessage("SetCardState", Card.Card_state.被遮挡, SendMessageOptions.DontRequireReceiver);
        }
        // 遍历所有的卡片，并交换他们的位置
        foreach (GameObject card in cards)
        {
            // 获取一个卡片的位置
            Transform cardTransform = card.transform;
            // 获取另一个随机卡片的位置
            Transform randomCardTransform = cards[Random.Range(0, cards.Length)].transform;
            // 交换两个卡片的位置
            Vector3 tempPosition = cardTransform.localPosition;
            cardTransform.localPosition = randomCardTransform.localPosition;
            randomCardTransform.localPosition = tempPosition;
        }
    }
}
