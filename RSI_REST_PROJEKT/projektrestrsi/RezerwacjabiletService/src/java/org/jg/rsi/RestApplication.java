package org.jg.rsi;

import java.util.HashSet;
import java.util.Set;
import javax.ws.rs.ApplicationPath;
import javax.ws.rs.core.Application;

@ApplicationPath("/")
public class RestApplication extends Application {
    @Override
    public Set<Class<?>> getClasses() {
        Set<Class<?>> resources = new HashSet<>();
        resources.add(HelloWorldImpl.class);
                resources.add(LoggingFilter.class);  // <-- DODAJ TUTAJ LoggingFilter
        resources.add(BasicAuthFilter.class); // <-- dodaj filtr zabezpieczeń
                //resources.add(GenericExceptionMapper.class);  // <-- dodaj tę klasę tutaj
        return resources;
    }
}
