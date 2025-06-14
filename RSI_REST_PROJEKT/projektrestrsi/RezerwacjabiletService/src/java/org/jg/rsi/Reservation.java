package org.jg.rsi;

import java.util.List;

public class Reservation {
    private String filmTitle;
    private String data;
    private String godzina;
    private List<String> miejsca;

    public Reservation() {}

    public Reservation(String filmTitle, String data, String godzina, List<String> miejsca) {
        this.filmTitle = filmTitle;
        this.data = data;
        this.godzina = godzina;
        this.miejsca = miejsca;
    }

    public String getFilmTitle() { return filmTitle; }
    public void setFilmTitle(String filmTitle) { this.filmTitle = filmTitle; }

    public String getData() { return data; }
    public void setData(String data) { this.data = data; }

    public String getGodzina() { return godzina; }
    public void setGodzina(String godzina) { this.godzina = godzina; }

    public List<String> getMiejsca() { return miejsca; }
    public void setMiejsca(List<String> miejsca) { this.miejsca = miejsca; }
}
