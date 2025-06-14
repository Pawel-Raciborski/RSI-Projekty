package org.jg.rsi;

import javax.annotation.Priority;
import javax.ws.rs.container.ContainerRequestContext;
import javax.ws.rs.container.ContainerRequestFilter;
import javax.ws.rs.core.HttpHeaders;
import javax.ws.rs.core.Response;

import java.io.IOException;
import java.nio.charset.StandardCharsets;
import java.util.Base64;
import java.util.StringTokenizer;
import javax.ws.rs.Priorities;
import javax.ws.rs.ext.Provider;

@Provider
@Priority(Priorities.AUTHENTICATION)
public class BasicAuthFilter implements ContainerRequestFilter {

    private static final String AUTHENTICATION_SCHEME = "Basic";

    // Tutaj na sztywno przykładowe dane do logowania
    private static final String USERNAME = "user";
    private static final String PASSWORD = "password";

    @Override
    public void filter(ContainerRequestContext requestContext) throws IOException {
        // Pobierz nagłówek Authorization
        String authorizationHeader = requestContext.getHeaderString(HttpHeaders.AUTHORIZATION);

        if (authorizationHeader == null || !authorizationHeader.startsWith(AUTHENTICATION_SCHEME + " ")) {
            abortWithUnauthorized(requestContext);
            return;
        }

        // Extract credentials from header
        String encodedCredentials = authorizationHeader.substring(AUTHENTICATION_SCHEME.length()).trim();
        String credentials = new String(Base64.getDecoder().decode(encodedCredentials), StandardCharsets.UTF_8);

        // credentials = username:password
        final StringTokenizer tokenizer = new StringTokenizer(credentials, ":");
        final String username = tokenizer.hasMoreTokens() ? tokenizer.nextToken() : "";
        final String password = tokenizer.hasMoreTokens() ? tokenizer.nextToken() : "";

        if (!validateUser(username, password)) {
            abortWithUnauthorized(requestContext);
        }
    }

    private boolean validateUser(String username, String password) {
        // Tu możesz zrobić np. zapytanie do bazy, albo prostą walidację
        return USERNAME.equals(username) && PASSWORD.equals(password);
    }

    private void abortWithUnauthorized(ContainerRequestContext requestContext) {
        // Odmowa dostępu z 401 i nagłówkiem WWW-Authenticate
        requestContext.abortWith(
            Response.status(Response.Status.UNAUTHORIZED)
                .header(HttpHeaders.WWW_AUTHENTICATE, AUTHENTICATION_SCHEME + " realm=\"RezerwacjaRealm\"")
                .entity("Access denied for this resource.")
                .build());
    }
}
