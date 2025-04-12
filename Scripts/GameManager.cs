using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Texture2D CursorTexture;

    private void Start()
    {
        // ���������
        Vector2 mousePos = Vector2.zero;
        UnityEngine.Cursor.SetCursor(CursorTexture, mousePos, CursorMode.Auto);
    }

    // ���ҿ�Ƭ
    public void ShuffleCards()
    {
        // ��ȡ���п�Ƭ����
        GameObject[] cards = GameObject.FindGameObjectsWithTag("Card");
        foreach (GameObject card in cards)
        {
            card.SendMessage("SetCardState", Card.Card_state.���ڵ�, SendMessageOptions.DontRequireReceiver);
        }
        // �������еĿ�Ƭ�����������ǵ�λ��
        foreach (GameObject card in cards)
        {
            // ��ȡһ����Ƭ��λ��
            Transform cardTransform = card.transform;
            // ��ȡ��һ�������Ƭ��λ��
            Transform randomCardTransform = cards[Random.Range(0, cards.Length)].transform;
            // ����������Ƭ��λ��
            Vector3 tempPosition = cardTransform.localPosition;
            cardTransform.localPosition = randomCardTransform.localPosition;
            randomCardTransform.localPosition = tempPosition;
        }
    }
}
