using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Inventory_Management_System.Application.Common.Response;

/// <summary>
/// Response wrapper class
/// </summary>
/// <typeparam name="T"></typeparam>
public class ResponseVm<T>
{
    public T Data { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public List<ResponseMessage> ResponseMessages { get; set; }

    public ResponseVm()
    {
    }

    public ResponseVm(T data)
    {
        Data = data;
    }
}
