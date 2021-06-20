package com.kwetter.Messaging;

import javax.enterprise.context.ApplicationScoped;
import javax.enterprise.event.Observes;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.kwetter.Models.Profile;
import com.rabbitmq.client.Channel;
import com.rabbitmq.client.Connection;
import com.rabbitmq.client.ConnectionFactory;
import com.rabbitmq.client.DeliverCallback;

import io.quarkus.runtime.StartupEvent;
import lombok.Getter;
import lombok.Setter;

//@Startup
//@Singleton
@ApplicationScoped
public class RabbitSubscriber {

    //@Inject
    //ConnectionFactory factory;

    private final String QUEUE_NAME = "profiles";
    private final String EXCHANGE_NAME = "deletion";

    void onStart(@Observes StartupEvent ev){
        try {
            OpenChannel();
            OpenDeletionChannel();
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    public void OpenChannel() throws Exception {
        ConnectionFactory factory = new ConnectionFactory();

        // TODO: Environment Development/Production
        factory.setHost("localhost");
        Connection connection = factory.newConnection();
        Channel channel = connection.createChannel();

        channel.queueDeclare(QUEUE_NAME, false, false, false, null);
        System.out.println(" [*] Waiting for messages. To exit press CTRL+C");

        DeliverCallback deliverCallback = (consumerTag, delivery) -> {
            String jsonMessage = new String(delivery.getBody(), "UTF-8");

            Message message = new ObjectMapper().readValue(jsonMessage, Message.class);
            
            Profile.add(message.userID, message.username);

            // TODO: Threading?
            //CreateProfile(message);

            System.out.println(" [x] Received '" + jsonMessage + "'");
        };
        channel.basicConsume(QUEUE_NAME, true, deliverCallback, consumerTag -> { });
    }

    public void OpenDeletionChannel() throws Exception {
        ConnectionFactory factory = new ConnectionFactory();

        // TODO Environments
        factory.setHost("localhost");
        Connection connection = factory.newConnection();
        Channel channel = connection.createChannel();
    
        channel.exchangeDeclare(EXCHANGE_NAME, "fanout");

        String queueName = channel.queueDeclare().getQueue();
        channel.queueBind(queueName, EXCHANGE_NAME, "");
    
        System.out.println(" [*] Waiting for messages. To exit press CTRL+C");
    
        DeliverCallback deliverCallback = (consumerTag, delivery) -> {
            String username = new String(delivery.getBody(), "UTF-8");

            Profile.remove(username);

            System.out.println(" [x] Received '" + username + "'");
        };
        channel.basicConsume(queueName, true, deliverCallback, consumerTag -> { });
    }

    static class Message{
        @Getter
        @Setter
        public String username;
    
        @Getter
        @Setter
        public String userID;

        public Message(){}

/* 
        Message(String username, String userID){
            this.username = username;
            this.userID = userID;
        } */
    }

/*     @Transactional
    private void CreateProfile(String message){
        Profile newProfile = new Profile();
        newProfile.firstName = "";
        newProfile.lastName = "";
        newProfile.biography = "";
        newProfile.userID = message;
        newProfile.persist();
    } */
}
