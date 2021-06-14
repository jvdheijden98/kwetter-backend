package com.kwetter.Controllers;

import java.util.List;

import javax.inject.Inject;
import javax.ws.rs.GET;
import javax.ws.rs.POST;
import javax.ws.rs.PUT;
import javax.ws.rs.Path;
import javax.ws.rs.Produces;
import javax.ws.rs.core.MediaType;
import javax.ws.rs.core.Response;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.JsonMappingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.kwetter.Models.Profile;

import org.jboss.resteasy.annotations.jaxrs.PathParam;

import lombok.Getter;
import lombok.Setter;

@Path("api/profile")
public class ProfileController {

    // Create
/*     @POST
    @Produces(MediaType.APPLICATION_JSON)
    @Path("new")
    public Response create(){

        
        return Response.status(201).entity("hello").build();
    } */

    static class Message{

        @Getter
        @Setter
        public String username;

        public Message(){}
    }
    
    static class UpdateRequest {

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

        public UpdateRequest(){}
    }

    @POST
    @Path("/read")
    @Produces(MediaType.APPLICATION_JSON)
    public Profile Read(Message request) {
        String username = request.username;
        Profile profile = Profile.findByUsername(username);

        //System.out.println("username is this: " + username);
        //Message message = new ObjectMapper().readValue(username, Message.class);
        //System.out.println("dusername is this: " + message.username);
        //Profile profile = Profile.findByUsername(message.username);
        //System.out.println("Profile object is this: "+ profile);
        //return Response.status(200).entity(profile).build();

        return profile;
    }

    // TODO JWT Authorize
    @PUT
    @Path("/update")
    public Response Update(UpdateRequest request){
        Profile.updateProfile(request.firstName, request.lastName, request.biography, request.username);
        return Response.status(200).build();
    }

    // Read All - For Development purposes
    @GET
    @Path("/readall")
    @Produces(MediaType.APPLICATION_JSON)
    public Response ReadAll() {
        List<Profile> profiles = Profile.listAll();
        return Response.status(200).entity(profiles).build();
    }

    @GET
    @Produces(MediaType.TEXT_PLAIN)
    public String hello() {
        return "Hello RESTEasy!!";
    }
}