package org.jg.rsi;
import java.util.stream.Collectors;
import javax.activation.DataHandler;
import javax.activation.FileDataSource;
import javax.xml.ws.soap.MTOM;

import java.util.*;
@MTOM
public class Database {
    public static List<Film> FILMS = new ArrayList<>();

    public static Map<String, Map<String, Reservation>> USER_RESERVATIONS = new HashMap<>();

static {
    FILMS.add(new Film("Incepcja", "2025-05-10", "18:00", 
        new ArrayList<>(Arrays.asList("A1", "A2", "B1")), 
        createDataHandlerForImage("C:\\Users\\Dawid\\source\\repos\\Rezerwacjabilet\\Rezerwacjabilet\\wwwroot\\incepcja.jpg")));

    FILMS.add(new Film("Matrix", "2025-05-11", "20:00", 
        new ArrayList<>(Arrays.asList("A1", "A2", "B1")), 
        createDataHandlerForImage("C:\\Users\\Dawid\\source\\repos\\Rezerwacjabilet\\Rezerwacjabilet\\wwwroot\\matrix.jpg")));

    FILMS.add(new Film("Titanic", "2025-05-12", "17:30", 
        new ArrayList<>(Arrays.asList("A1", "A2", "B1")), 
        createDataHandlerForImage("C:\\Users\\Dawid\\source\\repos\\Rezerwacjabilet\\Rezerwacjabilet\\wwwroot\\titanic.jpg")));
}

private static DataHandler createDataHandlerForImage(String path) {
    try {
        FileDataSource fds = new FileDataSource(path);
        return new DataHandler(fds);
    } catch (Exception e) {
        e.printStackTrace();
        return null;
    }
}

    public static Film getFilm(String title) {
        return FILMS.stream().filter(f -> f.getTytul().equals(title)).findFirst().orElse(null);
    }

    public static String reserveSeats(String userId, String title, List<String> seats) {
        Film film = getFilm(title);
        if (film == null) return "Film not found.";


        if (!film.getDostepneMiejsca().containsAll(seats)) {
            return "Some seats are already taken: " + seats.stream()
                .filter(s -> !film.getDostepneMiejsca().contains(s))
        .collect(Collectors.toList());
        }

        film.getDostepneMiejsca().removeAll(seats);

        USER_RESERVATIONS.putIfAbsent(userId, new HashMap<>());
        Map<String, Reservation> reservations = USER_RESERVATIONS.get(userId);
        reservations.put(title, new Reservation(title, film.getData(), film.getGodzina(), seats));

        return "Reservation successful.";
    }

    public static String cancelReservation(String userId, String title) {
        Map<String, Reservation> reservations = USER_RESERVATIONS.get(userId);
        if (reservations == null) return "No reservation found for user.";

        Reservation r = reservations.remove(title);
        if (r != null) {
            Film film = getFilm(title);
            if (film != null) film.getDostepneMiejsca().addAll(r.getMiejsca());
            return "Reservation cancelled.";
        }
        return "No reservation found.";
    }

    public static String modifyReservation(String userId, String title, List<String> newSeats) {
        Film film = getFilm(title);
        if (film == null) return "Film not found.";

        Map<String, Reservation> reservations = USER_RESERVATIONS.get(userId);
        if (reservations == null) return "No reservation found for user.";

        Reservation oldRes = reservations.get(title);
        List<String> available = new ArrayList<>(film.getDostepneMiejsca());
        if (oldRes != null) {
            available.addAll(oldRes.getMiejsca());
        }

if (!available.containsAll(newSeats)) {
    return "Some new seats are already taken: " +
        newSeats.stream()
               .filter(s -> !available.contains(s))
               .collect(Collectors.toList());
}
        if (oldRes != null) {
            film.getDostepneMiejsca().addAll(oldRes.getMiejsca());
        }

        film.getDostepneMiejsca().removeAll(newSeats);

        reservations.put(title, new Reservation(title, film.getData(), film.getGodzina(), newSeats));
        return "Reservation modified.";
    }

    public static List<Reservation> getUserReservations(String userId) {
        Map<String, Reservation> reservations = USER_RESERVATIONS.get(userId);
        if (reservations == null) return new ArrayList<>();
        return new ArrayList<>(reservations.values());
    }
}
