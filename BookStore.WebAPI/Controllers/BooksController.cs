using BookStore.WebAPI.Core.OtherObjects;
using Microsoft.AspNetCore.Authorization;
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


    [HttpGet("Getbooks")]
    public ActionResult<string> Get()
    {
        return Ok(Classics);
    }

    /// <summary>
    /// Retrieve data with user authorization
    /// </summary>
    /// <returns>[string]</returns>

    [HttpGet("GetUserRole")]
    [Authorize(Roles =StaticUserRoles.USER)]
    public ActionResult<string> GetUserRole()
    {
        return Ok(Classics);
    }

    /// <summary>
    /// Getting data with admin authorization
    /// </summary>
    /// <returns>[string]</returns>

    [HttpGet("GetAdminRole")]
    [Authorize(Roles = StaticUserRoles.ADMIN)]
    public ActionResult<string> GetAdminRole()
    {
        return Ok(Classics);
    }

    /// <summary>
    /// Retrieve data with owner authorization
    /// </summary>
    /// <returns>[string]</returns>

    [HttpGet("GetOwnerRole")]
    [Authorize(Roles = StaticUserRoles.OWNER)]
    public ActionResult<string> GetOwnerRole()
    {
        return Ok(Classics);
    }
}