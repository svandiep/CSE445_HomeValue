using System.ServiceModel;
using System.ServiceModel.Web;

namespace HomeValue
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ValueByZip?zip={zip}")]
        int ValueByZip(string zip);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "/CityAverage?city={city}&state={state}")]
        int CityAverage(string city, string state);
        // TODO: Add your service operations here
    }   
}
