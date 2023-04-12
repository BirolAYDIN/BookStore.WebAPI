using Microsoft.AspNetCore.Mvc;

namespace BookStore.WebAPI.Controllers;


[Route("[controller]")]
[ApiController]
public class BookController : ControllerBase
{
    private static readonly string[] Classics = new[]
    {
        "1. Hayvan Çiftliği",
        "2. Bilinmeyen Bir Kadının Mektubu",
        "3. Suç ve Ceza",
        "4. Satranç",
        "5. Küçük Prens",
        "6. Dönüşüm",
        "7. Amok Koşucusu",
        "8. İnsan Neyle Yaşar?",
        "9. Fareler ve İnsanlar",
        "10. 1984",
        "11. Olağanüstü Bir Gece",
        "12. Bir Kadının Yaşamından Yirmi Dört Saat," +
        "13. Yeraltından Notlar",
        "14. Yabancı",
        "15. Genç Werther'in Acıları",
        "16. Bir İdam Mahkûmunun Son Günü",
        "17. Dorian Gray'in Portresi",
        "18. Korku",
        "19. Beyaz Zambaklar Ülkesi",
        "20. Otomatik Portakal"

};
    /// <summary>
    /// 20 World Classic Book
    /// </summary>
    /// <returns>[string]</returns>

    [Route("Getbooks")]
    [HttpGet]
    public ActionResult<string> Get()
    {
        return Ok(Classics);
    }
}