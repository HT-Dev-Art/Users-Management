namespace DevArt.Users.Application.Dto;

public class HttpRequestMessageOptionDto
{
    public HttpMethod Method { get; set; }
    
    public string Route { get; set; }
    
    public Object Body { get; set; }
    
    public bool AttachAuthorizationHeader { get; set; }
}