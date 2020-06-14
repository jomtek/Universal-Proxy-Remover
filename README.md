

# Jomtek's Universal Proxy-Remover
Unfortunately, at the moment, this proxy fixer can only fix proxies which return literal values.
For example, this proxy won't get fixed :

    static int myProxy() {
    	return 1 + 1;
    } 

But I'm doing my best to update it asap :)

# What is a proxy ?
When we're talking about IL code and .NET reverse engineering, a "proxy" is an intermediate method to a value.
Quick example

    static void main(string[] args) {
    	Console.WriteLine(myProxy());
    }
    
    static int myProxy() {
    	return "hello!;
    }


As you can see, proxies can make values harder to read. Now imagine this applied to thousands of values, submersed in thousands of lines of code...

# Proxy depth
Obviously, proxies can call other proxies, which will then call other proxies... That's why I added a "depth" setting on this proxy remover. Quick example of a depth 2 proxified code. 

    static void main(string[] args) {
	    Console.WriteLine(myProxy());
    }
    
    static int myProxy() {
	    return anotherProxy();
    }
    
    static int anotherProxy() {
	    return "hello!";
    }

<br>Result after proxy removal :

    static void main(string[] args) {
	    Console.WriteLine("hello!");
    }

# Credits
Special thanks to
- the NetShields Obfuscator community
- [0x4d4](https://github.com/0xd4d) (dnlib's inventor)
- [Mindsystemm](https://github.com/MindSystemm) (a nice friend of mine)
