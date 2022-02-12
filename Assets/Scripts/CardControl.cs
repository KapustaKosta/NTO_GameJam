using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardControl : MonoBehaviour
{
    [SerializeField] private Camera camera;

    private Card nowCard; 
    [SerializeField] private GameObject card;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject canvas;
    [SerializeField] private Transform cardOriginPoint;
    [SerializeField] private Image answerTextPanel;
    [SerializeField] private Text answerText;
    [SerializeField] private Text cardText;
    [SerializeField] private Text personNameText;
    [SerializeField] private Text personPositionText;
    [SerializeField] private Image personImage;
    [SerializeField] private Sprite menImage;
    [SerializeField] private Sprite wifeImage;
    [SerializeField] private Sprite doctorImage;
    [SerializeField] private Sprite daughterImage;
    [SerializeField] private Text moneyText;
    public float extremeDistance;

    private Vector2 rightExtremePoint;
    private Vector2 leftExtremePoint;
    bool isTouch;

    private string nowModifier;
    private List<Card> cards;

    public float money = 50;
    public float health = 50;
    public float friends = 50;
    public float income = 10;

    int notRepeatedCardsCount;

    void Start()
    {
        SetMoneyText();
        isTouch = false;
        notRepeatedCardsCount = 0;
        rightExtremePoint = card.transform.position + new Vector3(extremeDistance, 0, 0);
        leftExtremePoint = card.transform.position - new Vector3(extremeDistance, 0, 0);
        cards = Cards.getCards();
        foreach(Card c in cards)
        {
            if (c.mayRepeat) notRepeatedCardsCount++;
        }

        card = Instantiate(cardPrefab, canvas.transform);
        card.transform.position = cardOriginPoint.position;
        Transform answerPanel = card.transform.GetChild(0);
        answerPanel.gameObject.TryGetComponent<Image>(out answerTextPanel);
        Transform textPanel = answerPanel.transform.GetChild(0);
        textPanel.gameObject.TryGetComponent<Text>(out answerText);
        card.transform.GetChild(2).gameObject.TryGetComponent<Image>(out personImage);

        Card nextCard = null;

        foreach (Card c in cards)
        {
            if (c.modifierCondition == nowModifier)
            {
                nextCard = c;
                nowModifier = "";
                break;
            }
        }
        if (nextCard == null)
        {
            bool res = false;
            while (!res)
            {
                nextCard = cards[Random.Range(0, cards.Count - 1)];
                res = true;
                if (!nextCard.mayRepeat && nextCard.used)
                {
                    res = false;
                    continue;
                }
                if (nextCard.modifierCondition != "")
                {
                    switch (nextCard.modifierCondition)
                    {
                        case "здоровье < 50":
                            {
                                if (health > 50)
                                {
                                    res = false;
                                }
                                break;
                            }
                        case "Все карточки закончились (кроме повторяющихся) и Деньги < начального значения":
                            {
                                if (money > 50 && notRepeatedCardsCount > 0)
                                {
                                    res = false;
                                }
                                break;
                            }
                        case "Все карточки закончились (кроме повторяющихся) и Деньги >= начального значения.":
                            {
                                if (money < 50 && notRepeatedCardsCount > 0)
                                {
                                    res = false;
                                }
                                break;
                            }
                        case "Random.Next(100) > 90":
                            {
                                int a = Random.RandomRange(0, 100);
                                if (a < 90)
                                {
                                    res = false;
                                }
                                break;
                            }
                    }
                }
                if (res && !nextCard.mayRepeat && !nextCard.used)
                {
                    nextCard.used = true;
                }
            }
        }
        cardText.text = nextCard.text;
        switch (nextCard.person)
        {
            case Person.men:
                {
                    personImage.sprite = menImage;
                    personPositionText.text = "Я";
                    personNameText.text = "Артём Горюнов";
                    break;
                }
            case Person.wife:
                {
                    personImage.sprite = wifeImage;
                    personPositionText.text = "Жена";
                    personNameText.text = "Ева Емельянова";
                    break;
                }
            case Person.doctor:
                {
                    personImage.sprite = doctorImage;
                    personPositionText.text = "Доктор";
                    personNameText.text = "Даниил Коновалов";
                    break;
                }
        }
        nowCard = nextCard;
}

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPosWorld = camera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, camera.nearClipPlane));
            if (touchPosWorld.x > card.transform.position.x - 1f && touchPosWorld.x < card.transform.position.x + 1f && touchPosWorld.y > card.transform.position.y - 1f &&
                touchPosWorld.y < card.transform.position.y + 1f) isTouch = true;
            else isTouch = false;
            if (!isTouch) return;
            Debug.Log(touchPosWorld);
            if (touchPosWorld.x < rightExtremePoint.x && touchPosWorld.x > leftExtremePoint.x)
            {
                float deltaPosition = touchPosWorld.x - card.transform.position.x;
                card.transform.position += new Vector3(deltaPosition * 0.5f, 0, 0);
                float deltaRotation = -deltaPosition;
                card.transform.rotation = Quaternion.Euler(0, 0, card.transform.rotation.eulerAngles.z + deltaRotation);
                float alpha = (Mathf.Abs(touchPosWorld.x) / 2f) * 1.5f;
                if (alpha > 1) alpha = 1f;
                Debug.Log(alpha);
                Color textPanelColor = answerTextPanel.color;
                textPanelColor.a = alpha;
                answerTextPanel.color = textPanelColor;
                if (touchPosWorld.x > 0) answerText.text = "Да";
                else answerText.text = "Нет";
                Color textColor = answerText.color;
                textPanelColor.a = alpha * 1.5f;
                answerText.color = textPanelColor;
            }
            else
            {
                Destroy(card);
                Condition condition = null;
                if (touchPosWorld.x > 0)
                {
                    condition = nowCard.yesCondition;
                }
                else condition = nowCard.noCondition;
                money += condition.money;
                health += condition.health;
                friends += condition.friends;
                income += condition.income;
                nowModifier = condition.modifier;
                SetMoneyText();

                card = Instantiate(cardPrefab, canvas.transform);
                card.transform.position = cardOriginPoint.position;
                Transform answerPanel = card.transform.GetChild(0);
                answerPanel.gameObject.TryGetComponent<Image>(out answerTextPanel);
                Transform textPanel = answerPanel.transform.GetChild(0);
                textPanel.gameObject.TryGetComponent<Text>(out answerText);
                card.transform.GetChild(2).gameObject.TryGetComponent<Image>(out personImage);

                Card nextCard = null;

                foreach (Card c in cards)
                {
                    if (c.modifierCondition == nowModifier)
                    {
                        nextCard = c;
                        nowModifier = "";
                        break;
                    }
                }
                if (nextCard == null)
                {
                    bool res = false;
                    while (!res)
                    {
                        nextCard = cards[Random.Range(0, cards.Count - 1)];
                        res = true;
                        if (!nextCard.mayRepeat && nextCard.used)
                        {
                            res = false;
                            continue;
                        }
                        if (nextCard.modifierCondition != "")
                        {
                            switch (nextCard.modifierCondition)
                            {
                                case "здоровье < 50":
                                    {
                                        if (health > 50)
                                        {
                                            res = false;
                                        }
                                        break;
                                    }
                                case "Все карточки закончились (кроме повторяющихся) и Деньги < начального значения":
                                    {
                                        if (money > 50 && notRepeatedCardsCount > 0)
                                        {
                                            res = false;
                                        }
                                        break;
                                    }
                                case "Все карточки закончились (кроме повторяющихся) и Деньги >= начального значения.":
                                    {
                                        if (money < 50 && notRepeatedCardsCount > 0)
                                        {
                                            res = false;
                                        }
                                        break;
                                    }
                                case "Random.Next(100) > 90":
                                    {
                                        int a = Random.RandomRange(0, 100);
                                        if (a < 90)
                                        {
                                            res = false;
                                        }
                                        break;
                                    }
                            }
                        }
                        if (res && !nextCard.mayRepeat && !nextCard.used)
                        {
                            nextCard.used = true;
                        }
                    }
                }
                cardText.text = nextCard.text;
                switch (nextCard.person)
                {
                    case Person.men:
                        {
                            personImage.sprite = menImage;
                            personPositionText.text = "Я";
                            personNameText.text = "Артём Горюнов";
                            break;
                        }
                    case Person.wife:
                        {
                            personImage.sprite = wifeImage;
                            personPositionText.text = "Жена";
                            personNameText.text = "Ева Горюнова";
                            break;
                        }
                    case Person.doctor:
                        {
                            personImage.sprite = doctorImage;
                            personPositionText.text = "Доктор";
                            personNameText.text = "Даниил Коновалов";
                            break;
                        }
                    case Person.daughter:
                        {
                            personImage.sprite = daughterImage;
                            personPositionText.text = "Дочь";
                            personNameText.text = "Дарья Горюнова";
                            break;
                        }
                }
                nowCard = nextCard;
            }
        }
        else isTouch = false;
    }

    private void SetMoneyText()
    {
        moneyText.text = Mathf.RoundToInt(money).ToString();
    }
}
