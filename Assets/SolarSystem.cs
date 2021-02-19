﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SolarSystem : MonoBehaviour
{
    public GameObject star;
    public GameObject asteroid;

    public int[] radius;
    public List<GameObject> planetList = new List<GameObject>();
    public List<GameObject> orbitList = new List<GameObject>();

    Vector3 starScale = new Vector3(10, 10, 10);
    Vector3 planetScale = new Vector3(5, 5, 5);
    Vector3 asteroidScale = new Vector3(4, 4, 4);

    // Start is called before the first frame update
    void Start()
    {
        // move the objects into a resource file/ini/etc

        GameObject go = Instantiate(star, new Vector3(0, 0, 0), Quaternion.identity);
        go.name = "Star";
        go.transform.localScale = starScale;
        Rigidbody sunRB = go.GetComponent<Rigidbody>();
        sunRB.mass = Constants.MASS_SOL;
        sunRB.velocity = new Vector3(0, 0, 0);

        // hard coded for now
        int numPlanets = 4;

        System.Random myRand = new System.Random();
        for (int p = 0; p < numPlanets; p++)
        {
            double pAngle = -1 * myRand.NextDouble() * Constants.TWO_PI;
            int pPlus1 = p + 1;
            double radius = 100 * pPlus1 + 50 * p * myRand.NextDouble();
            // we have orbital references every 100 units, so make sure the actual orbits don't start on top of the reference lines
            if (radius % 100 < 10) { radius += 10; }
            GameObject planet = Instantiate(planetList[p], new Vector3((float)radius * (float)Math.Cos(pAngle), 0, -1 * (float)radius * (float)Math.Sin(pAngle)), Quaternion.identity);
            planet.name = "Planet" + pPlus1;
            planet.transform.localScale = planetScale;
            Rigidbody planetRB = planet.GetComponent<Rigidbody>();
            planetRB.mass = Constants.MASS_EARTH * (float)Math.Pow(pPlus1, 2);
            //Debug.Log("Planet: " + p + "   Mass: " + planetRB.mass);
            float v = (float)Math.Sqrt(Constants.G * Constants.G_SCALE / Math.Abs(radius));
            planetRB.velocity = new Vector3(v * (float)Math.Sin(pAngle), 0, v * (float)Math.Cos(pAngle));
            /*
             * v = sqrt ( G M / R )
             * if r = 100:
             * v = sqrt ( (2*Pi)^2 * 1 / 100 )
             * our radius unit is the AU which is equal to 100 pixels on our screen
             * v = sqrt ( 2*Pi / 10 )
             * G is (2*Pi)^2 in units: AU^3 MSol^-1 YEAR^-2
             */

            //create some orbit lines as a circular reference to compare our planetary orbits to.
            GameObject orbit = Instantiate(orbitList[p], new Vector3(0, 0, 0), Quaternion.identity);
            orbit.name = "Orbit"+pPlus1;
            orbit.SendMessage("SetRadius", 100*pPlus1);
        }

        GameObject goast = Instantiate(asteroid, new Vector3(-340, 0, -400), Quaternion.identity);
        goast.name = "Asteroid";
        goast.transform.localScale = asteroidScale;
        Rigidbody astRB = goast.GetComponent<Rigidbody>();
        astRB.mass = Constants.MASS_EARTH / 10;
        //Debug.Log("Asteroid mass: " + astRB.mass);
        astRB.velocity = new Vector3(0.4f * (float)Math.Sqrt(Constants.G_SCALE), 0, 0.3f * (float)Math.Sqrt(Constants.G_SCALE));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Plane XZPlane = new Plane(Vector3.up, Vector3.zero);
            Vector3 hitPoint = new Vector3();
            hitPoint = Vector3.zero;
            float distance;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (XZPlane.Raycast(ray, out distance))
            {
                hitPoint = ray.GetPoint(distance);
                //Just double check to ensure the y position is exactly zero
                hitPoint.y = 0;
            }

            GameObject goast2 = GameObject.Find("Asteroid");
            Destroy(goast2);

            Debug.Log("mouseDown at x z: " + hitPoint.x + " " + hitPoint.z);

            GameObject goast = Instantiate(asteroid, hitPoint, Quaternion.identity);
            goast.name = "Asteroid";
            goast.transform.localScale = asteroidScale;
            Rigidbody astRB = goast.GetComponent<Rigidbody>();
            //The asteroid is the same mass as the planets for added fun.
            //Note that due to floating point limitations, asteroids have to be no less than 1/10 of earth mass.
            //You could generate one with less mass but it would not make for a very interesting deflection, i.e. no effect.
            astRB.mass = Constants.MASS_EARTH / 1f;
            //Debug.Log("Asteroid mass: " + astRB.mass);
            float xVelUnscaled = 0.5f;
            float zVelUnscaled = 0.5f;
            if (hitPoint.x >= 0)
            {
                xVelUnscaled = -xVelUnscaled;
            }
            if (hitPoint.z >= 0)
            {
                zVelUnscaled = -zVelUnscaled;
            }

            astRB.velocity = new Vector3(xVelUnscaled * (float)Math.Sqrt(Constants.G_SCALE), 0, zVelUnscaled* (float)Math.Sqrt(Constants.G_SCALE));
        }
    }

        void OnGUI()
    {
        //Make a proper esc key menu, but for now...
        GUI.Label(new Rect(10, 10, 300, 20), "Click the screen to reposition the asteroid.");
    }
}