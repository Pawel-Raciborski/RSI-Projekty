package org.jg.rsi;
import java.io.IOException;
import javax.jws.WebService;
import java.util.List;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.logging.Level;
import java.util.logging.Logger;
import javax.activation.DataHandler;
import javax.activation.FileDataSource;
import javax.jws.HandlerChain;
import javax.jws.soap.SOAPBinding;
import javax.jws.soap.SOAPBinding.Style;
import javax.jws.soap.SOAPBinding.Use;
import javax.xml.ws.soap.MTOM;
@MTOM
@WebService
@SOAPBinding(style = Style.DOCUMENT, use = Use.LITERAL)
@HandlerChain(file = "handler-chain.xml")
public class HelloWorldImpl implements HelloWorld {

    
    
    @Override
    public String getHelloWorldAsString(String name) {
        return "Witaj świecie JAX-WS: " + name;
    }

    @Override
    public List<Film> getMovies() {
        List<Film> lista = new ArrayList<>();

        DataHandler img1 = new DataHandler(new FileDataSource("C:\\Users\\Dawid\\source\\repos\\Rezerwacjabilet\\Rezerwacjabilet\\wwwroot\\incepcja.jpg"));
        DataHandler img2 = new DataHandler(new FileDataSource("C:\\Users\\Dawid\\source\\repos\\Rezerwacjabilet\\Rezerwacjabilet\\wwwroot\\matrix.jpg"));
        DataHandler img3 = new DataHandler(new FileDataSource("C:\\Users\\Dawid\\source\\repos\\Rezerwacjabilet\\Rezerwacjabilet\\wwwroot\\titanic.jpg"));
        
       lista.add(new Film("Incepcja", "2025-05-10", "18:00", Arrays.asList("A1", "A2", "B1"), img1));
        lista.add(new Film("Matrix", "2025-05-11", "20:00", Arrays.asList("C1", "C2", "C3"), img2));
        lista.add(new Film("Titanic", "2025-05-12", "17:30", Arrays.asList("D1", "D2"), img3));

        try {
            System.out.println("Adding film with image size: " + img1.getDataSource().getInputStream().available());
        } catch (IOException ex) {
            Logger.getLogger(HelloWorldImpl.class.getName()).log(Level.SEVERE, null, ex);
        }
        return lista;
    }
}
