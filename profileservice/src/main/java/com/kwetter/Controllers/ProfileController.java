package com.kwetter.Controllers;

import javax.inject.Inject;
import javax.ws.rs.GET;
import javax.ws.rs.POST;
import javax.ws.rs.Path;
import javax.ws.rs.Produces;
import javax.ws.rs.core.MediaType;
import javax.ws.rs.core.Response;

import org.jboss.resteasy.annotations.jaxrs.PathParam;

@Path("/profile")
public class ProfileController {

    // Create
    @POST
    @Produces(MediaType.APPLICATION_JSON)
    @Path("new")
    public Response create(){

        
        return Response.status(201).entity("hello").build();
    }

    @GET
    @Produces(MediaType.TEXT_PLAIN)
    public String hello() {
        return "Hello RESTEasy!!";
    }
}