<Query Kind="Program" />

void Main()
{
	// Se crea un cliente y se le asignan algunas cuentas.
	Cliente cliente = new Cliente(123, "Fulano");
	cliente.AgregarCuenta(new CuentaAhorro("01234", "Cuenta ahorro", 2000M, 0.0001M));
	cliente.AgregarCuenta(new PrestamoFiduiciario("56789", "Préstamo", 1000000M, 15M, 600));
	TarjetaCredito tarjetaCredito = new TarjetaCredito("0123456789012345", "Tarjeta titular", 300M, 10);
	tarjetaCredito.AgregarTarjetaAdicional(new TarjetaCredito("1234567890123456", "Tarjeta adicional 1", 0M, 14));
	tarjetaCredito.AgregarTarjetaAdicional(new TarjetaCredito("2345678901234567", "Tarjeta adicional 2", 5000M, 20));
	cliente.AgregarCuenta(tarjetaCredito);
	cliente.Dump();
	
	// Se asigna el objeto cliente a copiaFalsa. Esto NO es una  copia, sólo se está creando un puntero nuevo al mismo objeto.
	Cliente copiaFalsa = cliente;
	
	// Se asigna una copia de cliente a copiaReal.
	Cliente copiaReal = (Cliente)cliente.Clonar();
	
	// Se agrega una cuenta más a cliente para ver cómo afecta a las copias.
	cliente.AgregarCuenta(new CuentaMonetaria("45678", "Cuenta monetaria", 1000M));
	tarjetaCredito.AgregarTarjetaAdicional(new TarjetaCredito("0000111122223333", "Otra tarjeta", 500M, 25));
	
	// Se muestran los datos de copiaFalsa y copiaReal. copiaFalsa muestra la cuenta y la tarjeta adicionales.
	copiaFalsa.Dump();
	copiaReal.Dump();
}

// La interfaz para crear copias es muy sencilla, sólo declara una función para crear copias del objeto.
public interface IPrototipo
{
	IPrototipo Clonar();
}

// La clase Cliente implementa la interfaz IPrototipo.
public class Cliente : IPrototipo
{
	private readonly List<Cuenta> cuentas;
	
	public Cliente(int codigo, string nombre)
	{
		Console.WriteLine($"Construyendo el cliente {nombre}...");
		this.cuentas = new List<Cuenta>();
		this.Codigo = codigo;
		this.Nombre = nombre;
	}
	
	//  La implementación de la copia puede variar. En este caso se declara un constructor privado que recibe un objeto del mismo tipo y
	// copia todos sus valores.
	private Cliente(Cliente cliente) : this(cliente.Codigo, cliente.Nombre)
	{
		Console.WriteLine($"Construyendo el cliente {cliente.Nombre} con el constructor privado...");
		
		// Dependiendo de qué tan completa se desee la copia, se pudo hacer this.cuentas = cliente.cuentas pero eso sólo copia el puntero al
		// objeto lista y no la lista o sus elementos individuales.
		foreach (Cuenta cuenta in cliente.Cuentas)
		{
			// La clase Cuenta también implementa IPrototipo por lo que se puede clonar. El casteo es porque Clonar() retorna IPrototipo.
			this.AgregarCuenta((Cuenta)cuenta.Clonar());
		}
	}
	
	public int Codigo { get; private set; }
	
	public string Nombre { get; private set; }
	
	public IReadOnlyCollection<Cuenta> Cuentas => cuentas.AsReadOnly();
	
	public void AgregarCuenta(Cuenta cuenta) => this.cuentas.Add(cuenta);
	
	// Clonar() sólo llama al constructor privado pasando esta misma instancia como parámetro para que el constructor privado la copie.
	public IPrototipo Clonar()
	{
		Console.WriteLine($"Clonando el cliente {this.Nombre}...");
		return new Cliente(this);
	}
}

// La clase Cuenta también implementa IPrototipo.
public abstract class Cuenta : IPrototipo
{
	public Cuenta(string numero, string nombre, decimal saldo)
	{
		Console.WriteLine($"Construyendo la cuenta {numero}...");
		this.Numero = numero;
		this.Nombre = nombre;
		this.Saldo = saldo;
	}

	public string Numero { get; private set; }
	
	public string Nombre { get; private set; }
	
	public decimal Saldo { get; private set; }
	
	// Aquí se implementa Clonar() de otra manera, aprovechando una función heredada de la clase Object. Sin embargo, sólo crea una "copia
	// superficial". Significa que sólo se copia "un nivel" de atributos de la clase. Por ese motivo se declara como virtual, en caso alguna
	// subclase necesite anular esta implementación.
	public virtual IPrototipo Clonar()
	{
		Console.WriteLine($"Clonando la cuenta {this.Numero} usando una copia superficial...");
		return (IPrototipo)this.MemberwiseClone();
	}
}

public class CuentaMonetaria : Cuenta
{
	public CuentaMonetaria(string numero, string nombre, decimal saldo) : base(numero, nombre, saldo)
	{
		Console.WriteLine($"Construyendo la cuenta monetaria {numero}...");
	}
}

public class CuentaAhorro : Cuenta
{
	public CuentaAhorro(string numero, string nombre, decimal saldo, decimal tasaInteres) : base(numero, nombre, saldo)
	{
		Console.WriteLine($"Construyendo la cuenta de ahorro {numero}...");
		this.TasaInteres = tasaInteres;
	}
	
	public decimal TasaInteres { get; private set; }
}

public class PrestamoFiduiciario : Cuenta
{
	public PrestamoFiduiciario(string numero, string nombre, decimal saldo, decimal tasaInteres, short cantidadCuotas)
		: base(numero, nombre, saldo)
	{
		Console.WriteLine($"Construyendo el préstamo fiduiciario {numero}...");
		this.TasaInteres = tasaInteres;
		this.CantidadCuotas = cantidadCuotas;
	}
	
	public decimal TasaInteres { get; private set; }
	
	public short CantidadCuotas { get; private set; }
}

public class TarjetaCredito : Cuenta
{
	private readonly List<TarjetaCredito> tarjetasAdicionales;
	
	public TarjetaCredito(string numero, string nombre, decimal saldo, byte fechaCorte) : base(numero, nombre, saldo)
	{
		Console.WriteLine($"Construyendo la tarjeta de crédito {numero}...");
		this.tarjetasAdicionales = new List<TarjetaCredito>();
		this.FechaCorte = fechaCorte;
	}
	
	private TarjetaCredito(TarjetaCredito tarjetaCredito)
		: this(tarjetaCredito.Numero, tarjetaCredito.Nombre, tarjetaCredito.Saldo, tarjetaCredito.FechaCorte)
	{
		Console.WriteLine($"Construyendo la tarjeta de crédito {tarjetaCredito.Numero} con el constructor privado...");
		foreach (TarjetaCredito tarjetaAdicional in tarjetaCredito.TarjetasAdicionales)
		{
			this.AgregarTarjetaAdicional((TarjetaCredito)tarjetaAdicional.Clonar());
		}
	}
	
	public IReadOnlyCollection<TarjetaCredito> TarjetasAdicionales => this.tarjetasAdicionales.AsReadOnly();
	
	public byte FechaCorte { get; private set; }
	
	public void AgregarTarjetaAdicional(TarjetaCredito tarjetaAdicional)
	{
		this.tarjetasAdicionales.Add(tarjetaAdicional);
	}
	
	// Como esta clase tiene a su vez una lista, se vuelve a implementar el método Clonar() anulando la copia superficial de la clase
	// Cuenta.
	public override IPrototipo Clonar()
	{
		Console.WriteLine($"Clonando la tarjeta de crédito {this.Numero}...");
		return new TarjetaCredito(this);
	}
}