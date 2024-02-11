using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IntroText : MonoBehaviour
{
    public string wstep1 = "W �wiecie, kt�ry ju� dawno zapomnia�, co to znaczy �y� w pokoju, otaczaj� nas jedynie ruiny przesz�o�ci. Jest rok 2124. Dwadzie�cia lat temu, �wiat, jaki zna�a ludzko��, znikn�� w mgnieniu oka, poch�oni�ty przez seri� globalnych katastrof - wojen, kl�sk naturalnych i pandemii, kt�re doprowadzi�y do upadku cywilizacji. To, co pozosta�o, to postapokaliptyczna pustkowie, pe�na zniszczonych miast, opuszczonych fabryk i rozpadaj�cych si� domostw, gdzie ka�dy dzie� to walka o przetrwanie.\r\nJeste� jednym z nielicznych ocala�ych. Twoim domem jest teraz bezlitosny �wiat, gdzie ka�dy krok mo�e by� ostatnim. Wok� Ciebie tylko pustka i ruiny, a w powietrzu unosi si� ci�ki zapach spalenizny i �mierci. Przemierzaj�c opuszczone ulice, walczysz o ka�dy k�s jedzenia, ka�d� kropl� wody. Twoim jedynym towarzyszem jest stary, zardzewia�y radiomagnetofon, kt�ry czasami �apie sygna�, przerywaj�c cisz� apokalipsy.\r\nTwoim celem jest przetrwanie. Ka�da decyzja, kt�r� podejmiesz, wp�ynie na Twoj� podr�. Czy dasz si� wci�gn�� w lokalne konflikty? Czy znajdziesz innych ocala�ych i spr�bujesz zbudowa� co� na nowo? A mo�e b�dziesz w�drowa� samotnie, unikaj�c ";
    public string wstep2 = "W �wiecie spustoszonym przez katastrof� nuklearn�, gdzie niebo zaciemnia wieczny popi�, a ziemia p�ka od promieniowania, przetrwanie sta�o si� codzienn� walk�. Jest rok 2124, a Ty jeste� jednym z nielicznych, kt�rzy przetrwali Zag�ad�. Twoim nowym domem jest bezkresna, zrujnowana pustynia, gdzie ka�dy skrawek cywilizacji jest tylko bladym wspomnieniem dawnych czas�w.\r\nW tej brutalnej rzeczywisto�ci w�drujesz samotnie, przemierzaj�c opuszczone miasta i zgliszcza, w poszukiwaniu zapas�w i schronienia. Twoje dni wype�nione s� ci�g�� walk� o przetrwanie. Musisz stawi� czo�a nie tylko trudom �rodowiska, ale tak�e innym desperackim ocala�ym i zmutowanym stworzeniom, kt�re teraz w��cz� si� po spustoszonym �wiecie.";
    public string wstep3 = "Witaj w roku 2124. �wiat, jaki zna�e�, nie istnieje. Po serii katastrofalnych wojen nuklearnych i ekologicznych kl�sk, cywilizacja ludzka leg�a w gruzach. Ziemia teraz jest nie do poznania, przemieniona w opuszczon� pustkowi� pe�n� radioaktywnych burz, zdzicza�ej przyrody i rozpadaj�cych si� ruin. Jeste� jednym z ocala�ych, desperacko walcz�cym o ka�dy kolejny dzie� w tym postapokaliptycznym koszmarze.\r\nTwoja podr� rozpoczyna si� w zrujnowanym mie�cie, gdzie ka�dy zau�ek kryje niebezpiecze�stwo. Musisz zdoby� jedzenie, wod�, lekarstwa i inne zasoby, aby przetrwa�. Wok� Ciebie kr��� inne osamotnione dusze - niekt�re przyjazne, wi�kszo�� jednak stanowi �miertelne zagro�enie. Po drodze natkniesz si� na przera�aj�ce mutanty i zdegenerowane maszyny, pozosta�o�ci po �wiatowym konflikcie.";

    public string[] hellos ;
    public float scrollSpeed = 20f; // Szybko�� przesuwania tekstu
    public Text text;

    // Use this for initialization
    void Start()
    {
        //text.text = hellos[0];
        //text.gameObject.SetActive(true);
        //Random.seed = (int)System.DateTime.Now.Ticks;
        //int randomIndex = Random.Range(0, 4);
        //text.text = hellos[randomIndex];
        hellos = new string[4] { wstep1, "Hola Mundo", wstep2, wstep3 };

        Random.seed = (int)System.DateTime.Now.Ticks;
        int randomIndex = Random.Range(0, hellos.Length);
        text.text = hellos[randomIndex];
        text.gameObject.SetActive(true);
    }
    void Update()
    {
        RectTransform rectTransform = text.GetComponent<RectTransform>();
        rectTransform.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Return))
        {

            // Ukryj tekst, gdy naci�ni�to Enter.
            text.gameObject.SetActive(false);
           
        }
    }
}
