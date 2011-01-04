using System;
using System.ComponentModel;
using System.Net;
using System.Runtime.Remoting;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web.Services.Protocols;

namespace CodeplexReleaseTool
{
  public interface ICodeplexWebService
  {
    /// <remarks/>
    event CreateProjectReleaseCompletedEventHandler CreateProjectReleaseCompleted;

    /// <remarks/>
    event CreateAReleaseCompletedEventHandler CreateAReleaseCompleted;

    /// <remarks/>
    event CreateReleaseCompletedEventHandler CreateReleaseCompleted;

    /// <remarks/>
    event CreateClickOnceReleaseCompletedEventHandler CreateClickOnceReleaseCompleted;

    /// <remarks/>
    event UpdateReleaseCompletedEventHandler UpdateReleaseCompleted;

    /// <remarks/>
    event UpdateClickOnceReleaseCompletedEventHandler UpdateClickOnceReleaseCompleted;

    /// <remarks/>
    event UploadReleaseFilesCompletedEventHandler UploadReleaseFilesCompleted;

    /// <remarks/>
    event UploadTheReleaseFilesCompletedEventHandler UploadTheReleaseFilesCompleted;

    /// <remarks/>
    event GetReleaseCompletedEventHandler GetReleaseCompleted;

    SoapProtocolVersion SoapVersion { get; set; }
    bool AllowAutoRedirect { get; set; }
    CookieContainer CookieContainer { get; set; }
    X509CertificateCollection ClientCertificates { get; }
    bool EnableDecompression { get; set; }
    string UserAgent { get; set; }
    IWebProxy Proxy { get; set; }
    bool UnsafeAuthenticatedConnectionSharing { get; set; }
    ICredentials Credentials { get; set; }
    bool UseDefaultCredentials { get; set; }
    string ConnectionGroupName { get; set; }
    bool PreAuthenticate { get; set; }
    string Url { get; set; }
    Encoding RequestEncoding { get; set; }
    int Timeout { get; set; }
    ISite Site { get; set; }
    IContainer Container { get; }

    /// <remarks/>
    [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.codeplex.com/services/ReleaseService/v1.0/CreateProjectRelease", RequestNamespace="http://www.codeplex.com/services/ReleaseService/v1.0", ResponseNamespace="http://www.codeplex.com/services/ReleaseService/v1.0", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
    int CreateProjectRelease(string projectName, string name, string description, string releaseDate, string status, bool showToPublic, bool isDefaultRelease, string username, string password, string changesetId);

    /// <remarks/>
    System.IAsyncResult BeginCreateProjectRelease(string projectName, string name, string description, string releaseDate, string status, bool showToPublic, bool isDefaultRelease, string username, string password, string changesetId, System.AsyncCallback callback, object asyncState);

    /// <remarks/>
    int EndCreateProjectRelease(System.IAsyncResult asyncResult);

    /// <remarks/>
    void CreateProjectReleaseAsync(string projectName, string name, string description, string releaseDate, string status, bool showToPublic, bool isDefaultRelease, string username, string password, string changesetId);

    /// <remarks/>
    void CreateProjectReleaseAsync(string projectName, string name, string description, string releaseDate, string status, bool showToPublic, bool isDefaultRelease, string username, string password, string changesetId, object userState);

    /// <remarks/>
    [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.codeplex.com/services/ReleaseService/v1.0/CreateARelease", RequestNamespace="http://www.codeplex.com/services/ReleaseService/v1.0", ResponseNamespace="http://www.codeplex.com/services/ReleaseService/v1.0", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
    int CreateARelease(string projectName, string name, string description, string releaseDate, string status, bool showToPublic, bool isDefaultRelease, string username, string password);

    /// <remarks/>
    System.IAsyncResult BeginCreateARelease(string projectName, string name, string description, string releaseDate, string status, bool showToPublic, bool isDefaultRelease, string username, string password, System.AsyncCallback callback, object asyncState);

    /// <remarks/>
    int EndCreateARelease(System.IAsyncResult asyncResult);

    /// <remarks/>
    void CreateAReleaseAsync(string projectName, string name, string description, string releaseDate, string status, bool showToPublic, bool isDefaultRelease, string username, string password);

    /// <remarks/>
    void CreateAReleaseAsync(string projectName, string name, string description, string releaseDate, string status, bool showToPublic, bool isDefaultRelease, string username, string password, object userState);

    /// <remarks/>
    [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.codeplex.com/services/ReleaseService/v1.0/CreateRelease", RequestNamespace="http://www.codeplex.com/services/ReleaseService/v1.0", ResponseNamespace="http://www.codeplex.com/services/ReleaseService/v1.0", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
    int CreateRelease(string projectName, string name, string description, string releaseDate, string status, bool showToPublic, bool showOnHomePage, bool isDefaultRelease, string username, string password);

    /// <remarks/>
    System.IAsyncResult BeginCreateRelease(string projectName, string name, string description, string releaseDate, string status, bool showToPublic, bool showOnHomePage, bool isDefaultRelease, string username, string password, System.AsyncCallback callback, object asyncState);

    /// <remarks/>
    int EndCreateRelease(System.IAsyncResult asyncResult);

    /// <remarks/>
    void CreateReleaseAsync(string projectName, string name, string description, string releaseDate, string status, bool showToPublic, bool showOnHomePage, bool isDefaultRelease, string username, string password);

    /// <remarks/>
    void CreateReleaseAsync(string projectName, string name, string description, string releaseDate, string status, bool showToPublic, bool showOnHomePage, bool isDefaultRelease, string username, string password, object userState);

    /// <remarks/>
    [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.codeplex.com/services/ReleaseService/v1.0/CreateClickOnceRelease", RequestNamespace="http://www.codeplex.com/services/ReleaseService/v1.0", ResponseNamespace="http://www.codeplex.com/services/ReleaseService/v1.0", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
    int CreateClickOnceRelease(string projectName, string name, string description, string releaseDate, string status, bool isDefaultRelease, string changesetId, [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")] byte[] clickOnceFileBytes, string username, string password);

    /// <remarks/>
    System.IAsyncResult BeginCreateClickOnceRelease(string projectName, string name, string description, string releaseDate, string status, bool isDefaultRelease, string changesetId, byte[] clickOnceFileBytes, string username, string password, System.AsyncCallback callback, object asyncState);

    /// <remarks/>
    int EndCreateClickOnceRelease(System.IAsyncResult asyncResult);

    /// <remarks/>
    void CreateClickOnceReleaseAsync(string projectName, string name, string description, string releaseDate, string status, bool isDefaultRelease, string changesetId, byte[] clickOnceFileBytes, string username, string password);

    /// <remarks/>
    void CreateClickOnceReleaseAsync(string projectName, string name, string description, string releaseDate, string status, bool isDefaultRelease, string changesetId, byte[] clickOnceFileBytes, string username, string password, object userState);

    /// <remarks/>
    [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.codeplex.com/services/ReleaseService/v1.0/UpdateRelease", RequestNamespace="http://www.codeplex.com/services/ReleaseService/v1.0", ResponseNamespace="http://www.codeplex.com/services/ReleaseService/v1.0", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
    void UpdateRelease(string projectName, int releaseId, string name, string description, string releaseDate, string status, bool showToPublic, bool isDefaultRelease, string changeSetId, string username, string password);

    /// <remarks/>
    System.IAsyncResult BeginUpdateRelease(string projectName, int releaseId, string name, string description, string releaseDate, string status, bool showToPublic, bool isDefaultRelease, string changeSetId, string username, string password, System.AsyncCallback callback, object asyncState);

    /// <remarks/>
    void EndUpdateRelease(System.IAsyncResult asyncResult);

    /// <remarks/>
    void UpdateReleaseAsync(string projectName, int releaseId, string name, string description, string releaseDate, string status, bool showToPublic, bool isDefaultRelease, string changeSetId, string username, string password);

    /// <remarks/>
    void UpdateReleaseAsync(string projectName, int releaseId, string name, string description, string releaseDate, string status, bool showToPublic, bool isDefaultRelease, string changeSetId, string username, string password, object userState);

    /// <remarks/>
    [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.codeplex.com/services/ReleaseService/v1.0/UpdateClickOnceRelease", RequestNamespace="http://www.codeplex.com/services/ReleaseService/v1.0", ResponseNamespace="http://www.codeplex.com/services/ReleaseService/v1.0", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
    void UpdateClickOnceRelease(string projectName, int releaseId, string name, string description, string releaseDate, string status, bool isDefaultRelease, string changeSetId, [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")] byte[] clickOnceFileBytes, string username, string password);

    /// <remarks/>
    System.IAsyncResult BeginUpdateClickOnceRelease(string projectName, int releaseId, string name, string description, string releaseDate, string status, bool isDefaultRelease, string changeSetId, byte[] clickOnceFileBytes, string username, string password, System.AsyncCallback callback, object asyncState);

    /// <remarks/>
    void EndUpdateClickOnceRelease(System.IAsyncResult asyncResult);

    /// <remarks/>
    void UpdateClickOnceReleaseAsync(string projectName, int releaseId, string name, string description, string releaseDate, string status, bool isDefaultRelease, string changeSetId, byte[] clickOnceFileBytes, string username, string password);

    /// <remarks/>
    void UpdateClickOnceReleaseAsync(string projectName, int releaseId, string name, string description, string releaseDate, string status, bool isDefaultRelease, string changeSetId, byte[] clickOnceFileBytes, string username, string password, object userState);

    /// <remarks/>
    [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.codeplex.com/services/ReleaseService/v1.0/UploadReleaseFiles", RequestNamespace="http://www.codeplex.com/services/ReleaseService/v1.0", ResponseNamespace="http://www.codeplex.com/services/ReleaseService/v1.0", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
    void UploadReleaseFiles(string projectName, string releaseName, ReleaseFile[] files, string username, string password);

    /// <remarks/>
    System.IAsyncResult BeginUploadReleaseFiles(string projectName, string releaseName, ReleaseFile[] files, string username, string password, System.AsyncCallback callback, object asyncState);

    /// <remarks/>
    void EndUploadReleaseFiles(System.IAsyncResult asyncResult);

    /// <remarks/>
    void UploadReleaseFilesAsync(string projectName, string releaseName, ReleaseFile[] files, string username, string password);

    /// <remarks/>
    void UploadReleaseFilesAsync(string projectName, string releaseName, ReleaseFile[] files, string username, string password, object userState);

    /// <remarks/>
    [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.codeplex.com/services/ReleaseService/v1.0/UploadTheReleaseFiles", RequestNamespace="http://www.codeplex.com/services/ReleaseService/v1.0", ResponseNamespace="http://www.codeplex.com/services/ReleaseService/v1.0", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
    void UploadTheReleaseFiles(string projectName, string releaseName, ReleaseFile[] files, string recommendedFileName, string username, string password);

    /// <remarks/>
    System.IAsyncResult BeginUploadTheReleaseFiles(string projectName, string releaseName, ReleaseFile[] files, string recommendedFileName, string username, string password, System.AsyncCallback callback, object asyncState);

    /// <remarks/>
    void EndUploadTheReleaseFiles(System.IAsyncResult asyncResult);

    /// <remarks/>
    void UploadTheReleaseFilesAsync(string projectName, string releaseName, ReleaseFile[] files, string recommendedFileName, string username, string password);

    /// <remarks/>
    void UploadTheReleaseFilesAsync(string projectName, string releaseName, ReleaseFile[] files, string recommendedFileName, string username, string password, object userState);

    /// <remarks/>
    [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.codeplex.com/services/ReleaseService/v1.0/GetRelease", RequestNamespace="http://www.codeplex.com/services/ReleaseService/v1.0", ResponseNamespace="http://www.codeplex.com/services/ReleaseService/v1.0", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
    Release GetRelease(string projectName, string releaseName, string username, string password);

    /// <remarks/>
    System.IAsyncResult BeginGetRelease(string projectName, string releaseName, string username, string password, System.AsyncCallback callback, object asyncState);

    /// <remarks/>
    Release EndGetRelease(System.IAsyncResult asyncResult);

    /// <remarks/>
    void GetReleaseAsync(string projectName, string releaseName, string username, string password);

    /// <remarks/>
    void GetReleaseAsync(string projectName, string releaseName, string username, string password, object userState);

    /// <remarks/>
    new void CancelAsync(object userState);

    void Discover ();
    void Abort ();
    void Dispose ();
    string ToString ();
    event EventHandler Disposed;
    object GetLifetimeService ();
    object InitializeLifetimeService ();
    ObjRef CreateObjRef (Type requestedType);
  }
}