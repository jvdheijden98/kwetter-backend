package com.kwetter.Models;

import javax.persistence.Cacheable;
import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.SequenceGenerator;

import io.quarkus.hibernate.orm.panache.PanacheEntity;
import lombok.Getter;
import lombok.Setter;

@Entity
@Cacheable
public class Profile extends PanacheEntity {

    @Getter
    @Setter
    public long ID;

    @Getter
    @Setter
    public String firstName;
    
    @Getter
    @Setter
    public String lastName;

    @Getter
    @Setter
    public String biography;

    public Profile(){

    }

    public Profile(int id, String firstName, String lastName, String biography){
        this.ID = id;
        this.firstName = firstName;
        this.lastName = lastName;
        this.biography = biography;
    }
    
}
