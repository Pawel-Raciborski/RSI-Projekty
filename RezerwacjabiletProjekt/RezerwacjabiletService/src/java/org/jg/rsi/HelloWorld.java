package org.jg.rsi;

import java.util.List;
import javax.jws.WebMethod;
import javax.jws.WebService;
import javax.jws.soap.SOAPBinding;
import javax.jws.soap.SOAPBinding.Style;
import javax.jws.soap.SOAPBinding.Use;
@WebService
@SOAPBinding(style = Style.DOCUMENT, use = Use.LITERAL)
public interface HelloWorld {

    @WebMethod
    String getHelloWorldAsString(String name);

    @WebMethod(operationName = "getFilmy")
    List<Film> getMovies();
    
}
