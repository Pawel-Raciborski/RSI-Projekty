package org.jg.rsi;

import javax.ws.rs.core.Response;
import javax.ws.rs.ext.ExceptionMapper;
import javax.ws.rs.ext.Provider;

import javax.json.Json;
import javax.json.JsonObject;

@Provider
public class GenericExceptionMapper implements ExceptionMapper<Throwable> {

    @Override
    public Response toResponse(Throwable exception) {
        // Możesz też logować wyjątek tutaj
        exception.printStackTrace();

        JsonObject errorJson = Json.createObjectBuilder()
            .add("error", "Internal server error")
            .add("message", exception.getMessage() != null ? exception.getMessage() : "Unknown error")
            .build();

        return Response.status(Response.Status.INTERNAL_SERVER_ERROR)
                .entity(errorJson.toString())
                .type("application/json")
                .build();
    }
}
