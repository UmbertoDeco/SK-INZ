using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IntroText : MonoBehaviour
{
    public string wstep1 = "W œwiecie, który ju¿ dawno zapomnia³, co to znaczy ¿yæ w pokoju, otaczaj¹ nas jedynie ruiny przesz³oœci. Jest rok 2124. Dwadzieœcia lat temu, œwiat, jaki zna³a ludzkoœæ, znikn¹³ w mgnieniu oka, poch³oniêty przez seriê globalnych katastrof - wojen, klêsk naturalnych i pandemii, które doprowadzi³y do upadku cywilizacji. To, co pozosta³o, to postapokaliptyczna pustkowie, pe³na zniszczonych miast, opuszczonych fabryk i rozpadaj¹cych siê domostw, gdzie ka¿dy dzieñ to walka o przetrwanie.\r\nJesteœ jednym z nielicznych ocala³ych. Twoim domem jest teraz bezlitosny œwiat, gdzie ka¿dy krok mo¿e byæ ostatnim. Wokó³ Ciebie tylko pustka i ruiny, a w powietrzu unosi siê ciê¿ki zapach spalenizny i œmierci. Przemierzaj¹c opuszczone ulice, walczysz o ka¿dy kês jedzenia, ka¿d¹ kroplê wody. Twoim jedynym towarzyszem jest stary, zardzewia³y radiomagnetofon, który czasami ³apie sygna³, przerywaj¹c ciszê apokalipsy.\r\nTwoim celem jest przetrwanie. Ka¿da decyzja, któr¹ podejmiesz, wp³ynie na Twoj¹ podró¿. Czy dasz siê wci¹gn¹æ w lokalne konflikty? Czy znajdziesz innych ocala³ych i spróbujesz zbudowaæ coœ na nowo? A mo¿e bêdziesz wêdrowaæ samotnie, unikaj¹c ";
    public string wstep2 = "W œwiecie spustoszonym przez katastrofê nuklearn¹, gdzie niebo zaciemnia wieczny popió³, a ziemia pêka od promieniowania, przetrwanie sta³o siê codzienn¹ walk¹. Jest rok 2124, a Ty jesteœ jednym z nielicznych, którzy przetrwali Zag³adê. Twoim nowym domem jest bezkresna, zrujnowana pustynia, gdzie ka¿dy skrawek cywilizacji jest tylko bladym wspomnieniem dawnych czasów.\r\nW tej brutalnej rzeczywistoœci wêdrujesz samotnie, przemierzaj¹c opuszczone miasta i zgliszcza, w poszukiwaniu zapasów i schronienia. Twoje dni wype³nione s¹ ci¹g³¹ walk¹ o przetrwanie. Musisz stawiæ czo³a nie tylko trudom œrodowiska, ale tak¿e innym desperackim ocala³ym i zmutowanym stworzeniom, które teraz w³ócz¹ siê po spustoszonym œwiecie.";
    public string wstep3 = "Witaj w roku 2124. Œwiat, jaki zna³eœ, nie istnieje. Po serii katastrofalnych wojen nuklearnych i ekologicznych klêsk, cywilizacja ludzka leg³a w gruzach. Ziemia teraz jest nie do poznania, przemieniona w opuszczon¹ pustkowiê pe³n¹ radioaktywnych burz, zdzicza³ej przyrody i rozpadaj¹cych siê ruin. Jesteœ jednym z ocala³ych, desperacko walcz¹cym o ka¿dy kolejny dzieñ w tym postapokaliptycznym koszmarze.\r\nTwoja podró¿ rozpoczyna siê w zrujnowanym mieœcie, gdzie ka¿dy zau³ek kryje niebezpieczeñstwo. Musisz zdobyæ jedzenie, wodê, lekarstwa i inne zasoby, aby przetrwaæ. Wokó³ Ciebie kr¹¿¹ inne osamotnione dusze - niektóre przyjazne, wiêkszoœæ jednak stanowi œmiertelne zagro¿enie. Po drodze natkniesz siê na przera¿aj¹ce mutanty i zdegenerowane maszyny, pozosta³oœci po œwiatowym konflikcie.";

    public string[] hellos ;
    public float scrollSpeed = 20f; // Szybkoœæ przesuwania tekstu
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

            // Ukryj tekst, gdy naciœniêto Enter.
            text.gameObject.SetActive(false);
           
        }
    }
}
