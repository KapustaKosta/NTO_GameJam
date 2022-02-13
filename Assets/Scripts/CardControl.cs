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
    [SerializeField] private GameObject gameOverPanel;
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
    [SerializeField] private Sprite colleagueImage;
    [SerializeField] private Sprite cookImage;
    [SerializeField] private Sprite judgeImage;
    [SerializeField] private Sprite friendImage;
    [SerializeField] private Text moneyText;
    public float extremeDistance;

    bool canPlay;
    private Vector2 rightExtremePoint;
    private Vector2 leftExtremePoint;
    bool isTouch;

    private string nowModifier = "";
    private List<Card> cards;

    public float money = 10000000;
    private float startMoney;
    public float health = 80;
    public float friends = 50;
    public float income = 100000;

    int notRepeatedCardsCount = 10;

    void Start()
    {
        startMoney = money;
        canPlay = true;
        SetMoneyText();
        isTouch = false;
        rightExtremePoint = card.transform.position + new Vector3(extremeDistance, 0, 0);
        leftExtremePoint = card.transform.position - new Vector3(extremeDistance, 0, 0);
        cards = Cards.getCards();

        card = Instantiate(cardPrefab, canvas.transform);
        card.transform.position = cardOriginPoint.position;
        Transform answerPanel = card.transform.GetChild(0);
        answerPanel.gameObject.TryGetComponent<Image>(out answerTextPanel);
        Transform textPanel = answerPanel.transform.GetChild(0);
        textPanel.gameObject.TryGetComponent<Text>(out answerText);
        Transform panel = card.transform.GetChild(1);
        panel.transform.GetChild(1).gameObject.TryGetComponent<Image>(out personImage);

        Card nextCard = GetNextCard();
        Debug.Log(nextCard.text);
        cardText.text = nextCard.text;
        SetPerson(nextCard);
        nowCard = nextCard;
        if (!nextCard.mayRepeat && !nextCard.used)
        {
            nextCard.used = true;
            notRepeatedCardsCount--;
        }
    }

    void Update()
    {
        if (Input.touchCount > 0 && canPlay)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPosWorld = camera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, camera.nearClipPlane));
            if (touchPosWorld.x > card.transform.position.x - 1.2f && touchPosWorld.x < card.transform.position.x + 1.2f && touchPosWorld.y > card.transform.position.y - 2f &&
                touchPosWorld.y < card.transform.position.y + 2f) isTouch = true;
            else isTouch = false;
            if (!isTouch) return;
            if (touchPosWorld.x < rightExtremePoint.x && touchPosWorld.x > leftExtremePoint.x)
            {
                float deltaPosition = touchPosWorld.x - card.transform.position.x;
                card.transform.position += new Vector3(deltaPosition * 0.5f, 0, 0);
                float deltaRotation = -deltaPosition;
                card.transform.rotation = Quaternion.Euler(0, 0, card.transform.rotation.eulerAngles.z + deltaRotation);
                float alpha = (Mathf.Abs(touchPosWorld.x) / 2f) * 1.5f;
                if (alpha > 1) alpha = 1f;
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
                if (!nowCard.mayRepeat && condition.modifier != "Суд с “Пиццерия3”" && condition.modifier != "Рекламная кампания против “Пиццерии3” проведена" && condition.modifier != "Участие в конкурсе" &&
                    condition.modifier != "Очная конференция"  && condition.modifier != "Онлайн конференция") notRepeatedCardsCount--;
                money += condition.money;
                health += condition.health;
                friends += condition.friends;
                income += condition.income;
                nowModifier = condition.modifier;
                money += income;
                health -= 1;
                SetMoneyText();

                card = Instantiate(cardPrefab, canvas.transform);
                card.transform.position = cardOriginPoint.position;
                Transform answerPanel = card.transform.GetChild(0);
                answerPanel.gameObject.TryGetComponent<Image>(out answerTextPanel);
                Transform textPanel = answerPanel.transform.GetChild(0);
                textPanel.gameObject.TryGetComponent<Text>(out answerText);
                Transform panel = card.transform.GetChild(1);
                panel.transform.GetChild(1).gameObject.TryGetComponent<Image>(out personImage);

                Card nextCard = GetNextCard();
                cardText.text = nextCard.text;
                SetPerson(nextCard);
                nowCard = nextCard;
                if (!nextCard.mayRepeat)
                {
                    nextCard.used = true;
                }
                Debug.Log(notRepeatedCardsCount);
                if (nowCard.modifierCondition.Contains("Финал"))
                {
                    gameOverPanel.SetActive(true);
                    canPlay = false;
                }
            }
        }
        else isTouch = false;
    }

    private Card GetNextCard()
    {
        Card nextCard = null;

        if (friends < 0)
        {
            nextCard = cards[27];
            nowCard = nextCard;
            cardText.text = nextCard.text;
            gameOverPanel.SetActive(true);
            canPlay = false;
            personImage.sprite = wifeImage;
            personPositionText.text = "Жена";
            personNameText.text = "Ева Горюнова";
            return nextCard;
        }

        if (money < 0)
        {
            nextCard = cards[28];
            nowCard = nextCard;
            cardText.text = nextCard.text;
            gameOverPanel.SetActive(true);
            canPlay = false;
            personImage.sprite = wifeImage;
            personPositionText.text = "Жена";
            personNameText.text = "Ева Горюнова";
            return nextCard;
        }

        if (health < 20)
        {
            nextCard = cards[29];
            nowCard = nextCard;
            cardText.text = nextCard.text;
            gameOverPanel.SetActive(true);
            canPlay = false;
            personImage.sprite = wifeImage;
            personPositionText.text = "Жена";
            personNameText.text = "Ева Горюнова";
            return nextCard;
        }


        foreach (Card c in cards)
        {
            if (c.modifierCondition.Length > 0 && c.modifierCondition == nowModifier)
            {
                nextCard = c;
                nowModifier = "";
                return nextCard;
            }
        }
        bool res = false;
        while (!res)
        {
            int index = Random.Range(0, cards.Count - 3);
            nextCard = cards[index];
            if(nowCard != null && nextCard.text == nowCard.text) continue;
            res = true;
            if (!nextCard.mayRepeat && nextCard.used)
            {
                res = false;
                continue;
            }
            if (nextCard.modifierCondition.Length > 0 && nextCard.modifierCondition != nowModifier && nextCard.modifierCondition != "Все карточки закончились (кроме повторяющихся) и Деньги < начального значения" &&
                nextCard.modifierCondition != "Все карточки закончились (кроме повторяющихся) и Деньги >= начального значения." && nextCard.modifierCondition != "здоровье < 50")
            {
                res = false;
                continue;
            }
            Debug.Log(nextCard.modifierCondition);
            if (nextCard.modifierCondition.Length > 0)
            {
                Debug.Log(nextCard.modifierCondition);
                switch (nextCard.modifierCondition)
                {
                    case "здоровье < 50":
                        {
                            if (health >= 50)
                            {
                                res = false;
                                continue;
                            }
                            break;
                        }
                    case "Все карточки закончились (кроме повторяющихся) и Деньги < начального значения":
                        {
                            Debug.Log("1: " + money + " " + startMoney + " " + notRepeatedCardsCount);
                            if (money >= startMoney || notRepeatedCardsCount > 0)
                            {
                                res = false;
                                continue;
                            }
                            break;
                        }
                    case "Все карточки закончились (кроме повторяющихся) и Деньги >= начального значения.":
                        {
                            Debug.Log("2: " + money + " " + startMoney + " " + notRepeatedCardsCount);
                            if (money < startMoney || notRepeatedCardsCount > 0)
                            {
                                res = false;
                                continue;
                            }
                            break;
                        }
                    case "Random.Next(100) > 90":
                        {
                            int a = Random.Range(0, 100);
                            if (a < 90)
                            {
                                res = false;
                                continue;
                            }
                            break;
                        }
                }
            }
            if (nowModifier.Length > 0 && nextCard.modifierCondition != nowModifier) res = false;
        }
        return nextCard;
    }

    private void SetPerson(Card chosenCard)
    {
        switch (chosenCard.person)
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
            case Person.colleague:
                {
                    personImage.sprite = colleagueImage;
                    personPositionText.text = "Коллега";
                    personNameText.text = "Арсений Захаров";
                    break;
                }
            case Person.cook:
                {
                    personImage.sprite = cookImage;
                    personPositionText.text = "Пицамейкер";
                    personNameText.text = "Аделина Демина";
                    break;
                }
            case Person.judge:
                {
                    personImage.sprite = judgeImage;
                    personPositionText.text = "Судья";
                    personNameText.text = "Ксения Артемова";
                    break;
                }
            case Person.friend:
                {
                    personImage.sprite = friendImage;
                    personPositionText.text = "Друг";
                    personNameText.text = "Даниил Исаев";
                    break;
                }
        }
    }

    private void SetMoneyText()
    {
        moneyText.text = Mathf.RoundToInt(money).ToString();
    }
}
