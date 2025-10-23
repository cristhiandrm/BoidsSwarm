using UnityEngine;
using System.Collections.Generic;

public class Boid : MonoBehaviour
{
    public SwarmManager manager;
    private Vector3 velocity;

    void Start()
    {
        velocity = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        ).normalized * manager.maxSpeed;
    }

    void Update()
    {
        Vector3 acceleration = Vector3.zero;

        Vector3 avoidanceVector = CalculateObstacleAvoidance();
        acceleration += avoidanceVector * manager.obstacleWeight;

        acceleration += CalculateTargetSteering() * manager.targetWeight;

        if (avoidanceVector == Vector3.zero)
        {
            List<Boid> neighbors = manager.GetNeighborsUsingGrid(this);

            if (neighbors.Count > 0)
            {
                acceleration += CalculateCohesion(neighbors) * manager.cohesionWeight;
                acceleration += CalculateSeparation(neighbors) * manager.separationWeight;
                acceleration += CalculateAlignment(neighbors) * manager.alignmentWeight;
            }
        }

        acceleration += StayInBounds();

        velocity += acceleration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, manager.maxSpeed);
        transform.position += velocity * Time.deltaTime;

        if (velocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(velocity);
        }
    }

    private Vector3 CalculateCohesion(List<Boid> neighbors)
    {
        Vector3 centerOfMass = Vector3.zero;
        foreach (var neighbor in neighbors)
        {
            centerOfMass += neighbor.transform.position;
        }
        centerOfMass /= neighbors.Count;
        return (centerOfMass - transform.position).normalized;
    }

    private Vector3 CalculateSeparation(List<Boid> neighbors)
    {
        Vector3 separationVector = Vector3.zero;
        foreach (var neighbor in neighbors)
        {
            Vector3 direction = transform.position - neighbor.transform.position;
            separationVector += direction.normalized / (direction.magnitude + 0.0001f);
        }
        return separationVector.normalized;
    }

    private Vector3 CalculateAlignment(List<Boid> neighbors)
    {
        Vector3 averageVelocity = Vector3.zero;
        foreach (var neighbor in neighbors)
        {
            averageVelocity += neighbor.GetVelocity();
        }
        averageVelocity /= neighbors.Count;
        return averageVelocity.normalized;
    }

    private Vector3 StayInBounds()
    {
        Vector3 steer = Vector3.zero;
        if (transform.position.x > manager.bounds.x) steer.x = -1;
        if (transform.position.x < -manager.bounds.x) steer.x = 1;
        if (transform.position.y > manager.bounds.y) steer.y = -1;
        if (transform.position.y < -manager.bounds.y) steer.y = 1;
        if (transform.position.z > manager.bounds.z) steer.z = -1;
        if (transform.position.z < -manager.bounds.z) steer.z = 1;

        steer *= (transform.position.magnitude / manager.bounds.magnitude) * manager.boundsSteerForce;
        return steer;
    }

    private Vector3 CalculateObstacleAvoidance()
    {
        if (velocity == Vector3.zero) return Vector3.zero;

        if (Physics.SphereCast(
                transform.position, manager.boidRadius, velocity.normalized,
                out RaycastHit hit, manager.obstacleLookAhead, manager.obstacleLayer))
        {
            float forceScale = 1.0f - (hit.distance / manager.obstacleLookAhead);
            return hit.normal * forceScale;
        }
        return Vector3.zero;
    }

    private Vector3 CalculateTargetSteering()
    {
        if (manager.target == null)
        {
            return Vector3.zero;
        }
        return (manager.target.position - transform.position).normalized;
    }

    public Vector3 GetVelocity()
    {
        return velocity;
    }
}

