using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 开始新一轮的时间间隔
    public float Interval = 1.0f;
    public Texture2D CursorTexture;  // 鼠标显示的纹理

    // 私有成员
    // 每轮中玩家选中的卡片
    private GameObject[] cards; // 所有的卡片
    private GameObject[] SelectCards;
    // 记录每轮玩家选中了几张卡片
    private int SelectCount = 0;
    // 鼠标的MeshRenderer
    private MeshRenderer CursorMeshRenderer;
    private bool InputEnabled = true; // 是否输入

    private void Start()
    {
        // 设置鼠标光标
        Vector2 mousePos = Vector2.zero;
        UnityEngine.Cursor.SetCursor(CursorTexture, mousePos, CursorMode.Auto);
        // 获取鼠标的MeshRenderer
        CursorMeshRenderer = GameObject.Find("Cursor").GetComponent<MeshRenderer>();
        // 获取选择的卡片
        SelectCards = new GameObject[3];
        // 获取所有的卡片
        cards = GameObject.FindGameObjectsWithTag("Card");
        // 打乱卡片
        ShuffleCards();
    }

    private void Update()
    {
        HandleInput();
    }

    // 打乱卡片
    public void ShuffleCards()
    {
        foreach (GameObject card in cards)
        {
            card.SendMessage("SetState", Card.Card_state.被遮挡, SendMessageOptions.DontRequireReceiver);
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

    // 处理玩家输入
    public void HandleInput()
    {
        if (!InputEnabled || (!Input.GetMouseButtonDown(0)))
        {
            return;
        }
        // 获取鼠标点击的物体
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hit = Physics.RaycastAll(ray);
        // 遍历所有与激光相交的对象
        foreach(RaycastHit h in hit)
        {
            h.collider.SendMessage("SetState", Card.Card_state.显示, SendMessageOptions.DontRequireReceiver);
            UpdateTurn(h.collider.gameObject);
        }
    }
    // 更新玩家的选择
    public void UpdateTurn(GameObject card)
    {
        // 如果选择的卡片已经被选中，则不再处理
        for (int i = 0; i < SelectCards.Length; i++)
        {
            if (SelectCards[i] == card)
            {
                return;
            }
        }
        // 如果选择的卡片没有被选中，则将其加入到选择的卡片中
        SelectCards[SelectCount] = card;
        ++SelectCount;
        // 如果已选中两张卡片，则判断是否一致
        if (SelectCards[SelectCount - 1].GetComponent<MeshFilter>().mesh.name != SelectCards[SelectCount - 2].GetComponent<MeshFilter>().mesh.name)
        {
            // 重新开始下一轮
            StartCoroutine(LoseTurn());
            return;
        }
        //如果玩家当前选择的两张图片一致，则判断是否选中三张
        if (SelectCount >= 3)
        {
            // 重新开始下一轮
            StartCoroutine(WinTurn());
            return;
        }
    }

    // 启用或禁用输入
    public void EnableInput(bool enable = true)
    {
        CursorMeshRenderer.enabled = InputEnabled = enable;
    }

    // 选择卡片一致时调用
    public IEnumerator WinTurn()
    {
        // 禁止输入
        EnableInput(false);
        // 等待一段时间后重新开始
        yield return new WaitForSeconds(Interval);
        // 让玩家选择的卡片消失
        foreach (GameObject card in SelectCards)
        {
            if (card)
                card.SendMessage("SetState", Card.Card_state.隐藏, SendMessageOptions.DontRequireReceiver);
        }
        // 重置选择的卡片
        SelectCount = 0;
        // 清空选择的数组
        SelectCards = new GameObject[3];
        EnableInput(true);  // 启用输入
    }

    // 选择卡片不一致时调用
    public IEnumerator LoseTurn()
    {
        // 禁止输入
        EnableInput(false);
        // 等待一段时间后重新开始
        yield return new WaitForSeconds(Interval);
        // 让玩家选择的卡片消失
        foreach (GameObject card in SelectCards)
        {
            if(card)
            card.SendMessage("SetState", Card.Card_state.被遮挡, SendMessageOptions.DontRequireReceiver);
        }
        // 重置选择的卡片
        SelectCount = 0;
        // 清空选择的数组
        SelectCards = new GameObject[3];
        EnableInput(true);  // 启用输入
    }
}
