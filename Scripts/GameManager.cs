using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // ��ʼ��һ�ֵ�ʱ����
    public float Interval = 1.0f;
    public Texture2D CursorTexture;  // �����ʾ������

    // ˽�г�Ա
    // ÿ�������ѡ�еĿ�Ƭ
    private GameObject[] cards; // ���еĿ�Ƭ
    private GameObject[] SelectCards;
    // ��¼ÿ�����ѡ���˼��ſ�Ƭ
    private int SelectCount = 0;
    // ����MeshRenderer
    private MeshRenderer CursorMeshRenderer;
    private bool InputEnabled = true; // �Ƿ�����

    private void Start()
    {
        // ���������
        Vector2 mousePos = Vector2.zero;
        UnityEngine.Cursor.SetCursor(CursorTexture, mousePos, CursorMode.Auto);
        // ��ȡ����MeshRenderer
        CursorMeshRenderer = GameObject.Find("Cursor").GetComponent<MeshRenderer>();
        // ��ȡѡ��Ŀ�Ƭ
        SelectCards = new GameObject[3];
        // ��ȡ���еĿ�Ƭ
        cards = GameObject.FindGameObjectsWithTag("Card");
        // ���ҿ�Ƭ
        ShuffleCards();
    }

    private void Update()
    {
        HandleInput();
    }

    // ���ҿ�Ƭ
    public void ShuffleCards()
    {
        foreach (GameObject card in cards)
        {
            card.SendMessage("SetState", Card.Card_state.���ڵ�, SendMessageOptions.DontRequireReceiver);
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

    // �����������
    public void HandleInput()
    {
        if (!InputEnabled || (!Input.GetMouseButtonDown(0)))
        {
            return;
        }
        // ��ȡ�����������
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hit = Physics.RaycastAll(ray);
        // ���������뼤���ཻ�Ķ���
        foreach(RaycastHit h in hit)
        {
            h.collider.SendMessage("SetState", Card.Card_state.��ʾ, SendMessageOptions.DontRequireReceiver);
            UpdateTurn(h.collider.gameObject);
        }
    }
    // ������ҵ�ѡ��
    public void UpdateTurn(GameObject card)
    {
        // ���ѡ��Ŀ�Ƭ�Ѿ���ѡ�У����ٴ���
        for (int i = 0; i < SelectCards.Length; i++)
        {
            if (SelectCards[i] == card)
            {
                return;
            }
        }
        // ���ѡ��Ŀ�Ƭû�б�ѡ�У�������뵽ѡ��Ŀ�Ƭ��
        SelectCards[SelectCount] = card;
        ++SelectCount;
        // �����ѡ�����ſ�Ƭ�����ж��Ƿ�һ��
        if (SelectCards[SelectCount - 1].GetComponent<MeshFilter>().mesh.name != SelectCards[SelectCount - 2].GetComponent<MeshFilter>().mesh.name)
        {
            // ���¿�ʼ��һ��
            StartCoroutine(LoseTurn());
            return;
        }
        //�����ҵ�ǰѡ�������ͼƬһ�£����ж��Ƿ�ѡ������
        if (SelectCount >= 3)
        {
            // ���¿�ʼ��һ��
            StartCoroutine(WinTurn());
            return;
        }
    }

    // ���û��������
    public void EnableInput(bool enable = true)
    {
        CursorMeshRenderer.enabled = InputEnabled = enable;
    }

    // ѡ��Ƭһ��ʱ����
    public IEnumerator WinTurn()
    {
        // ��ֹ����
        EnableInput(false);
        // �ȴ�һ��ʱ������¿�ʼ
        yield return new WaitForSeconds(Interval);
        // �����ѡ��Ŀ�Ƭ��ʧ
        foreach (GameObject card in SelectCards)
        {
            if (card)
                card.SendMessage("SetState", Card.Card_state.����, SendMessageOptions.DontRequireReceiver);
        }
        // ����ѡ��Ŀ�Ƭ
        SelectCount = 0;
        // ���ѡ�������
        SelectCards = new GameObject[3];
        EnableInput(true);  // ��������
    }

    // ѡ��Ƭ��һ��ʱ����
    public IEnumerator LoseTurn()
    {
        // ��ֹ����
        EnableInput(false);
        // �ȴ�һ��ʱ������¿�ʼ
        yield return new WaitForSeconds(Interval);
        // �����ѡ��Ŀ�Ƭ��ʧ
        foreach (GameObject card in SelectCards)
        {
            if(card)
            card.SendMessage("SetState", Card.Card_state.���ڵ�, SendMessageOptions.DontRequireReceiver);
        }
        // ����ѡ��Ŀ�Ƭ
        SelectCount = 0;
        // ���ѡ�������
        SelectCards = new GameObject[3];
        EnableInput(true);  // ��������
    }
}
