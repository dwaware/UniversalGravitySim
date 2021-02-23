using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;

public class SolarSystem : MonoBehaviour
{
    private float alphaAmount = 1f;

    public GameObject star;
    public GameObject asteroid;

    public float asteroidMass = 3e-7f;

    public int[] radius;
    public List<GameObject> planetList = new List<GameObject>();
    public List<GameObject> orbitList = new List<GameObject>();

    Vector3 starScale = new Vector3(20, 20, 20);
    Vector3 planetScale = new Vector3(10, 10, 10);
    Vector3 asteroidScale = new Vector3(5, 5, 5);

    // clicking on the screen will result in a measurement of how long the mouse button was held down
    // this will trandlate into a "velocity" for a new asteroid the user will create when they release the mouse button
    // the initial velocity is:
    float newAsteroidVelocity = 0.1f;

    // holding the mouse button "charges" the velocity:
    float chargeSpeed = 0.5f;

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

        GameObject goast = Instantiate(asteroid, new Vector3(-1000, 0, -500), Quaternion.identity);
        goast.name = "Asteroid";
        goast.transform.localScale = asteroidScale;
        Rigidbody astRB = goast.GetComponent<Rigidbody>();
        astRB.mass = asteroidMass;
        astRB.velocity = new Vector3(0.4f * (float)Math.Sqrt(Constants.G_SCALE), 0, 0.3f * (float)Math.Sqrt(Constants.G_SCALE));
        newAsteroidVelocity = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        //Input.GetMouseButtonDown = when mouse button is clicked
        //Input.GetMouseButton = when mouse button is held down
        //Input.GetMouseButtonUp = on release

        //time for a new asteroid:
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            newAsteroidVelocity = 0.1f;
        }

        //let's charge the velocity:
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            newAsteroidVelocity += chargeSpeed * Time.deltaTime;
            newAsteroidVelocity = (newAsteroidVelocity > 2.5f ? 2.5f : newAsteroidVelocity);
        }


        //and create a new asteroid (after destroying the old one)
        if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("New asteroid velocity:  "+newAsteroidVelocity);

            if (GameObject.Find("Asteroid") != null)
            {
                GameObject goast2 = GameObject.Find("Asteroid");
                Destroy(goast2);
                Debug.Log("User has destroyed the old asteroid!");
            }
            else
            {
                //Debug.Log("Nothing to destroy!");
            }

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

            Debug.Log("New asteroid created at x,z: " + (int)hitPoint.x + "," + (int)hitPoint.z);

            GameObject goast = Instantiate(asteroid, hitPoint, Quaternion.identity);
            goast.name = "Asteroid";
            goast.transform.localScale = asteroidScale;
            Rigidbody astRB = goast.GetComponent<Rigidbody>();
            //The asteroid is the same mass as the planets for added fun.
            //Note that due to floating point limitations, asteroids have to be no less than 1/10 of earth mass.
            //You could generate one with less mass but it would not make for a very interesting deflection, i.e. no effect.
            astRB.mass = asteroidMass;
            //Debug.Log("Asteroid mass: " + astRB.mass);
            float xVelUnscaled = newAsteroidVelocity;
            float zVelUnscaled = newAsteroidVelocity;
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

        if (alphaAmount >= 0f)
        {
            alphaAmount -= 0.02f * Time.deltaTime;
        }

        GUI.color = new Color(1, 1, 1, alphaAmount);

        if (alphaAmount >= 0f)
        {
            GUI.Label(new Rect(10, 10, 400, 20), "Click the screen to reposition the asteroid.");
            GUI.Label(new Rect(10, 30, 800, 20), "The longer you hold the mouse button down, the faster your asteroid will travel!");
        }
    }

    public void OnSliderValueChanged(float value)
    {
        float sliderAsteroidMass = GameObject.Find("Slider_Mass").GetComponent<Slider>().value;
        this.asteroidMass = sliderAsteroidMass;

        if (GameObject.Find("Asteroid") != null)
        {
            Rigidbody astRB = GameObject.Find("Asteroid").GetComponent<Rigidbody>();
            astRB.mass = asteroidMass;
        }

        GameObject.Find("Text_Handle").GetComponent<Text>().text = sliderAsteroidMass.ToString() + " solar masses";
    }

    public void Reload()
    {
        Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
    }
}