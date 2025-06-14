package org.jg.rsi;

import java.util.ArrayList;
import java.util.List;
import javax.activation.DataHandler;
import javax.xml.bind.annotation.XmlMimeType;
import javax.xml.bind.annotation.XmlType;
import javax.xml.bind.annotation.XmlElement;

@XmlType(name = "Film")
public class Film {
    private String tytul;
    private String data;
    private String godzina;
    private List<String> dostepneMiejsca;
    private DataHandler zdjecie;
private List<Link> links = new ArrayList<>();

    public List<Link> getLinks() {
        return links;
    }

    public void setLinks(List<Link> links) {
        this.links = links;
    }
    public Film() {}

    public Film(String tytul, String data, String godzina, List<String> miejsca, DataHandler zdjecie) {
        this.tytul = tytul;
        this.data = data;
        this.godzina = godzina;
        this.dostepneMiejsca = miejsca;
        this.zdjecie = zdjecie;
    }

    // Gettery i settery
    public String getTytul() { return tytul; }
    public void setTytul(String tytul) { this.tytul = tytul; }

    public String getData() { return data; }
    public void setData(String data) { this.data = data; }

    public String getGodzina() { return godzina; }
    public void setGodzina(String godzina) { this.godzina = godzina; }

    @XmlElement(name = "miejsca")
    public List<String> getDostepneMiejsca() { return dostepneMiejsca; }
    public void setDostepneMiejsca(List<String> dostepneMiejsca) { this.dostepneMiejsca = dostepneMiejsca; }

    @XmlMimeType("application/octet-stream")
    public DataHandler getZdjecie() {
        return zdjecie;
    }

    public void setZdjecie(DataHandler zdjecie) {
        this.zdjecie = zdjecie;
    }
}
