package org.jg.rsi;

import javax.ws.rs.*;
import javax.ws.rs.core.MediaType;
import java.io.IOException;
import java.util.List;

@Path("/service") // REST endpoint
@Produces(MediaType.APPLICATION_JSON)
@Consumes(MediaType.APPLICATION_JSON)
public class HelloWorldImpl {

    @GET
@Path("/getFilmy")
public List<Film> getMovies() {
    List<Film> films = Database.FILMS;
    String baseUri = "http://localhost:8080/RezerwacjabiletService/service";

    for (Film f : films) {
        // Czyszczenie poprzednich linków, jeśli jakieś były
        f.getLinks().clear();

        // Dodaj link "self"
        f.getLinks().add(new Link("self", baseUri + "/getFilmy/" + f.getTytul()));

        // Dodaj link do rezerwacji
        f.getLinks().add(new Link("reserve", baseUri + "/makeReservation?filmTitle=" + f.getTytul()));
    }
    return films;
}


    @GET
    @Path("/getUserReservations")
    public List<Reservation> getUserReservations(@QueryParam("userId") String userId) {
        return Database.getUserReservations(userId);
    }

    @POST
    @Path("/makeReservation")
    public String makeReservation(@QueryParam("userId") String userId,
                                  @QueryParam("filmTitle") String filmTitle,
                                  List<String> seats) {
        return Database.reserveSeats(userId, filmTitle, seats);
    }

    @POST
    @Path("/cancelReservation")
    public String cancelReservation(@QueryParam("userId") String userId,
                                    @QueryParam("filmTitle") String filmTitle) {
        return Database.cancelReservation(userId, filmTitle);
    }

    @POST
    @Path("/modifyReservation")
    public String modifyReservation(@QueryParam("userId") String userId,
                                    @QueryParam("filmTitle") String filmTitle,
                                    List<String> newSeats) {
        return Database.modifyReservation(userId, filmTitle, newSeats);
    }
}
