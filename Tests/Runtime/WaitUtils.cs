using NUnit.Framework;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class WaitUtils
{
    public static float WaitUntilCooldown = 1f;

    static public IEnumerator WaitUntil(Func<bool> predicate, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        var cooldown = WaitUntilCooldown;

        while (cooldown > 0f)
        {
            cooldown -= Time.deltaTime;
            if (predicate())
            {
                break;
            }

            yield return null;
        }

        if (cooldown <= 0)
        {
            var fileName = System.IO.Path.GetFileName(filePath);
            Assert.Fail($"WaitUntil[in {fileName}:{lineNumber}] failed at cooldown.");
        }
    }
}
