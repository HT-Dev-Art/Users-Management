namespace DevArt.Users.Application.Dto;

public class HttpRequestMessageOptionDto<TBody>
{
    public HttpMethod Method { get; set; }
    
    public string Route { get; set; }
    
    public TBody Body { get; set; }
    
    public bool AttachAuthorizationHeader { get; set; }
}
