package com.kwetter.Models;

import javax.persistence.Cacheable;
import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.SequenceGenerator;
import javax.transaction.Transactional;

import io.quarkus.hibernate.orm.panache.PanacheEntity;
import lombok.Getter;
import lombok.Setter;

@Entity
@Cacheable
public class Profile extends PanacheEntity {

    // Panache provides an ID

    @Getter
    @Setter
    public String firstName;
    
    @Getter
    @Setter
    public String lastName;

    @Getter
    @Setter
    public String biography;

    @Getter
    @Setter
    public String username;

    @Getter
    @Setter
    public String userID;

    @Transactional
    public static void add(String userID, String username){
        Profile newProfile = new Profile();
        newProfile.firstName = "first name goes here";
        newProfile.lastName = "last name goes here";
        newProfile.biography = "your biography goes here";
        newProfile.username = username;
        newProfile.userID = userID;
        newProfile.persist();
    }

    @Transactional
    public static void updateProfile(String firstName, String lastName, String biography, String username){
        Profile changingProfile = findByUsername(username);
        if(!isNullOrWhiteSpace(firstName)){
            changingProfile.firstName = firstName;
        }
        if(!isNullOrWhiteSpace(biography)){
            changingProfile.biography = biography;
        }
        if(!isNullOrWhiteSpace(lastName)){
            changingProfile.lastName = lastName;
        }
        changingProfile.persist();
    }

    @Transactional
    public static void remove(String username){
        Profile toDeleteProfile = findByUsername(username);
        toDeleteProfile.delete();
    }

    public static Profile findByUsername(String username){
        return find("username", username).firstResult();
    }

    public static boolean isNullOrWhiteSpace(String value) {
        return value == null || value.trim().isEmpty();
    }

/* 
    public Profile(){

    }

    public Profile(String firstName, String lastName, String biography){
        this.firstName = firstName;
        this.lastName = lastName;
        this.biography = biography;
    } */
    
}
