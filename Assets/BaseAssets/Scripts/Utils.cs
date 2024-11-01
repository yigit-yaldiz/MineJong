using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class Utils
{
    public static Vector3 SetX(this Vector3 vector, float value)
    {
        vector.x = value;
        return vector;
    }
    public static Vector3 SetY(this Vector3 vector, float value)
    {
        vector.y = value;
        return vector;
    }
    public static Vector3 SetZ(this Vector3 vector, float value)
    {
        vector.z = value;
        return vector;
    }
    public static Vector3 Right(this Vector3 vector)
    {
        return new Vector3(vector.z, vector.y, -vector.x);
    }
    public static void ResetAll(this Transform transform)
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }
    public static void LocalResetAll(this Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }
    public static Vector3 Curve(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 p1 = Vector3.Lerp(a, b, t);
        Vector3 p2 = Vector3.Lerp(b, c, t);
        return Vector3.Lerp(p1, p2, t);
    }
    public static Vector3 CurveMoveTowords(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 p1 = Vector3.Lerp(a, b, t);
        Vector3 p2 = Vector3.Lerp(b, c, t);
        return Vector3.MoveTowards(p1, p2, t);
    }
    public static Vector3 CurveUnclamped(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 p1 = Vector3.LerpUnclamped(a, b, t);
        Vector3 p2 = Vector3.LerpUnclamped(b, c, t);
        return Vector3.LerpUnclamped(p1, p2, t);
    }
    public static string LimitedFloatWithDot(float value, int digit, bool removeDigitsWhenInt)
    {
        string result = value.ToString("F" + digit).Replace(",", ".");

        if (removeDigitsWhenInt && value % 1 == 0)
            result = value.ToString();

        return result;
    }
    public static float LimitDigits(float value, int digit)
    {
        float pow = Mathf.Pow(10, digit);
        value = Mathf.Round(value * pow) / pow;
        return value;
    }
    public static Color AddHSVToRGB(Color color, float h, float s, float v)
    {
        Color.RGBToHSV(color, out float currentH, out float currentS, out float currentV);

        return Color.HSVToRGB(currentH + h, currentS + s, currentV + v);
    }
    public static Quaternion LookAt(Vector3 position, Vector3 target, bool reverse = false, bool zeroY = true)
    {
        Vector3 direction;
        if (!reverse)
            direction = (target - position).normalized;
        else
            direction = (position - target).normalized;
        if (zeroY)
            direction.y = 0;
        if (direction == Vector3.zero)
            return Quaternion.identity;
        return Quaternion.LookRotation(direction);
    }
    public static Vector3 Direction(Vector3 position1, Vector3 position2, bool reverse = false, bool zeroY = true)
    {
        Vector3 direction;
        if (!reverse)
            direction = (position2 - position1).normalized;
        else
            direction = (position1 - position2).normalized;
        if (zeroY)
            direction.y = 0;
        return direction;
    }
    public static T GetClosestObject<T>(Vector3 origin, List<T> objects) where T : Object
    {
        T closestObject = null;
        float closestDistance = Mathf.Infinity;

        foreach (T t in objects)
        {
            GameObject obj = (t as GameObject);
            float distance = Vector3.Distance(origin, obj.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestObject = t;
            }
        }
        if (closestObject != null)
            return closestObject;
        else
            return null;
    }
    public static GameObject GetClosestObject(Vector3 origin, float radius, LayerMask layerMask)
    {
        GameObject closestObject = null;
        float closestDistance = Mathf.Infinity;
        Collider[] colliders = Physics.OverlapSphere(origin, radius, layerMask);

        foreach (Collider collider in colliders)
        {
            float distance = Vector3.Distance(origin, collider.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestObject = collider.gameObject;
            }
        }

        return closestObject;
    }
    public static GameObject[] GetClosestObjects(Vector3 origin, float radius, LayerMask layerMask)
    {
        Collider[] colliders = Physics.OverlapSphere(origin, radius, layerMask);
        GameObject[] closestObjects = new GameObject[colliders.Length];

        for (int i = 0; i < colliders.Length; i++)
        {
            closestObjects[i] = colliders[i].gameObject;
        }

        return closestObjects;
    }
    public static List<Vector3> GetPositionsInRadius(int count, float radius)
    {
        float angle = 360f / count;
        List<Vector3> positions = new List<Vector3>();

        for (int i = 0; i < count; i++)
        {
            positions.Add(new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle * i) * radius, 0, Mathf.Cos(Mathf.Deg2Rad * angle * i)) * radius);
        }

        return positions;
    }
    public static Vector3 ChangeVector(Vector3 defaultVector, Vector3 changeVector, bool x, bool y, bool z)
    {
        Vector3 newVector = new Vector3(
            x ? changeVector.x : defaultVector.x,
            y ? changeVector.y : defaultVector.y,
            z ? changeVector.z : defaultVector.z
            );

        return newVector;
    }
    public static float Angle(Vector3 direction1, Vector3 direction2)
    {
        float anlge = Vector3.Angle(direction1, direction2);
        return anlge;
    }
    public static float ReturnIn01Range(float start, float end, float value)
    {
        return Mathf.Clamp01((value - start) / (end - start));
    }
    public static (float, int) GetHighestFloat(float[] values)
    {
        List<float> valuesList = new List<float>(values);

        float highest = valuesList.Max();
        int index = 0;

        for (int i = 0; i < values.Length; i++)
        {
            if (values[i] == highest)
            {
                index = i;
            }
        }

        return (highest, index);
    }
    public static (float, int) GetLowestFloat(float[] values)
    {
        List<float> valuesList = new List<float>(values);

        float lowest = valuesList.Min();
        int index = 0;

        for (int i = 0; i < values.Length; i++)
        {
            if (values[i] == lowest)
            {
                index = i;
            }
        }

        return (lowest, index);
    }
    public static (float, int) GetAvarage(float[] values)
    {
        List<float> valuesList = new List<float>(values);

        float avarage = valuesList.Average();
        int index = 0;

        for (int i = 0; i < values.Length; i++)
        {
            if (values[i] == avarage)
            {
                index = i;
            }
        }

        return (avarage, index);
    }
    public static Vector3 MidPoint(List<Vector3> vectors)
    {
        Vector3 midPoint = Vector3.zero;
        for (int i = 0; i < vectors.Count; i++)
        {
            midPoint += vectors[i];
        }
        midPoint /= vectors.Count;
        return midPoint;
    }
    public static Vector3 SetAxis(Vector3 vector, char axis, float value)
    {
        return new Vector3((axis == 'x') ? value : vector.x, (axis == 'y') ? value : vector.y, (axis == 'z') ? value : vector.z);
    }
    public static T[] GetObjectsSphere<T>(Vector3 origin, float radius, int maxColliders, LayerMask layerMask) where T : Object
    {
        Collider[] results = new Collider[maxColliders];
        int c = Physics.OverlapSphereNonAlloc(origin, radius, results, layerMask);
        T[] closestObjects = new T[c];

        for (int i = 0; i < c; i++)
        {
            closestObjects[i] = results[i].transform.GetComponent<T>();
        }

        return closestObjects;
    }
    public static T[] GetObjectsBox<T>(Vector3 center, Vector3 halfExtents, Quaternion rotation, int maxColliders, LayerMask layerMask) where T : Object
    {
        Collider[] results = new Collider[maxColliders];
        int c = Physics.OverlapBoxNonAlloc(center, halfExtents, results, rotation, layerMask);

        T[] closestObjects = new T[c];

        for (int i = 0; i < c; i++)
        {
            closestObjects[i] = results[i].transform.GetComponent<T>();
        }

        return closestObjects;
    }

    #region OnlyInEditor
#if UNITY_EDITOR
    public static T SaveAsset<T>(T asset, string assetPath, bool overwrite = true, bool autoSaveAndRefresh = true) where T : Object
    {
        if (!overwrite)
        {
            assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
            AssetDatabase.CreateAsset(asset, assetPath);
        }
        else
        {
            T existingAsset = AssetDatabase.LoadAssetAtPath<T>(assetPath);

            if (existingAsset == null)
            {
                AssetDatabase.CreateAsset(asset, assetPath);
            }
            else
            {
                AssetDatabase.DeleteAsset(assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.CreateAsset(asset, assetPath);
                if (autoSaveAndRefresh)
                {
                    AssetDatabase.SaveAssets();
                }
                return existingAsset;
            }
        }
        if (autoSaveAndRefresh)
        {
            AssetDatabase.SaveAssets();
        }
        return asset;
    }
#endif
    #endregion
}
