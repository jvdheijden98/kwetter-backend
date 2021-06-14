package com.kwetter;

import io.quarkus.runtime.annotations.QuarkusMain;

import javax.inject.Inject;

import com.kwetter.Messaging.RabbitSubscriber;

import io.quarkus.runtime.Quarkus;
import io.quarkus.runtime.QuarkusApplication;

@QuarkusMain
public class Main {

    public static void main(String ... args) {

        Quarkus.run(Kwetter.class, args); 
    }

    public static class Kwetter implements QuarkusApplication {
        @Override
        public int run(String... args) throws Exception {
            System.out.println("Running main method!");

            Quarkus.waitForExit();
            return 0;
        }
    }
}
