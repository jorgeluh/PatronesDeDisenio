<Query Kind="Program" />

void Main()
{
	// Se puede usar el constructor directamente para crear un objeto ejecutando los pasos necesarios.
	IConstructor<Transferencia> constructorTransferencia = new ConstructorTransferencia();
	constructorTransferencia.FijarDatosBasicos("01234", "56789", 100M);
	constructorTransferencia.FijarBancoDestino(Banco.BancoA);
	Transferencia transferencia = constructorTransferencia.ObtenerResultado();
	transferencia.Dump();
	
	// Este es todo el proceso simplificado en métodos, pero se pierde flexibilidad.
	ProcesarTransferenciaAch(
		"01234", "56789", 0.01M, Banco.BancoC, "Prueba de ACH moneda cruzada.", Moneda.MonedaB);
	ProcesarTransferenciaAch(
		"01234", "56789", 1000000M, Banco.BancoC, "Prueba de ACH con autorizaciones.", autorizacionesRequeridas: 3);
}

// Se puede encapsular todo el proceso en un método, pero lo que busca este patrón es evitar tener una gran cantidad de parámetros pues
// muchos de ellos serán opcionales.
public bool ProcesarTransferenciaAch(
	string cuentaDebito,
	string cuentaCredito,
	decimal monto,
	Banco bancoDestino,
	string comentario = null,
	Moneda monedaDestino = Moneda.Local,
	byte autorizacionesRequeridas = 0)
{
	Director director = new Director();
	IConstructor<Transferencia> constructorTransferencia = new ConstructorTransferencia();
	IConstructor<ProcesadorTransferencia> constructorProcesador = new ConstructorProcesadorTransferencia();
	
	// Estas dos instrucciones crean objetos muy diferentes pero compatibles pues para ambos se ejecutan los mismos pasos del constructor.
	// No importa el tipo de resultado que el constructor genera, sólo que implemente la interfaz IConstructor que define los pasos
	// disponibles.
	Transferencia transferencia = director.ConstruirAch<Transferencia>(
		constructorTransferencia,
		cuentaDebito,
		cuentaCredito,
		monto,
		bancoDestino,
		comentario,
		monedaDestino,
		autorizacionesRequeridas);

	ProcesadorTransferencia procesador = director.ConstruirAch<ProcesadorTransferencia>(
		constructorProcesador,
		cuentaDebito,
		cuentaCredito,
		monto,
		bancoDestino,
		comentario,
		monedaDestino,
		autorizacionesRequeridas);
	
	return procesador.Procesar(transferencia);
}

// Esta es la interfaz del constructor que define los pasos y las opciones para "personalizar" el resultado.
public interface IConstructor<TResultado>
{
	void FijarDatosBasicos(string cuentaDebito, string cuentaCredito, decimal monto);
	
	void FijarComentario(string descripcion);
	
	void FijarBancoDestino(Banco banco);
	
	void FijarMonedaDestino(Moneda moneda);
	
	void FijarAutorizacionesRequeridas(byte autorizaciones);
	
	TResultado ObtenerResultado();
}

// Con este patrón es posible agregar una clase "director" que expone funciones que agrupan pasos para crear productos comunes. No es
// obligatoria, sólo tiene un papel auxiliar.
public class Director
{
	public T ConstruirLocal<T>(
		IConstructor<T> constructor,
		string cuentaDebito,
		string cuentaCredito,
		decimal monto,
		string comentario,
		Moneda monedaDestino,
		byte autorizacionesRequeridas)
	{
		constructor.FijarDatosBasicos(cuentaDebito, cuentaCredito, monto);
		constructor.FijarComentario(comentario);
		constructor.FijarMonedaDestino(monedaDestino);
		constructor.FijarAutorizacionesRequeridas(autorizacionesRequeridas);
		return constructor.ObtenerResultado();
	}
	
	public T ConstruirAch<T>(
		IConstructor<T> constructor,
		string cuentaDebito,
		string cuentaCredito,
		decimal monto,
		Banco bancoDestino,
		string comentario,
		Moneda monedaDestino,
		byte autorizacionesRequeridas)
	{
		constructor.FijarDatosBasicos(cuentaDebito, cuentaCredito, monto);
		constructor.FijarComentario(comentario);
		constructor.FijarBancoDestino(bancoDestino);
		constructor.FijarMonedaDestino(monedaDestino);
		constructor.FijarAutorizacionesRequeridas(autorizacionesRequeridas);
		return constructor.ObtenerResultado();
	}
}

// Este es un constructor concreto para crear transferencias.
public class ConstructorTransferencia : IConstructor<Transferencia>
{
	private Transferencia transferencia;
	
	public ConstructorTransferencia() => this.Reiniciar();

	public void FijarDatosBasicos(string cuentaDebito, string cuentaCredito, decimal monto)
	{
		Console.WriteLine("Fijando cuentas de débito, crédito y monto...");
		this.transferencia.FijarDatosBasicos(cuentaDebito, cuentaCredito, monto);
	}
	
	public void FijarComentario(string comentario)
	{
		Console.WriteLine("Fijando comentario...");
		this.transferencia.Comentario = comentario;
	}
	
	public void FijarBancoDestino(Banco banco)
	{
		Console.WriteLine("Fijando banco de destino, transferencia tipo ACH...");
		this.transferencia.BancoDestino = banco;
	}
	
	public void FijarMonedaDestino(Moneda moneda)
	{
		Console.WriteLine("Fijando moneda de destino, transferencia con moneda cruzada...");
		this.transferencia.MonedaDestino = moneda;
	}
	
	public void FijarAutorizacionesRequeridas(byte autorizaciones)
	{
		Console.WriteLine("Fijando la cantidad de autorizaciones...");
		this.transferencia.AutorizacionesRequeridas = autorizaciones;
	}
	
	// Este es el único método que permite obtener el producto una vez que se ha terminado de configurar.
	public Transferencia ObtenerResultado()
	{
		Transferencia transferencia = this.transferencia;
		Console.WriteLine("Entregando transferencia terminada...");
		this.Reiniciar();
		return transferencia;
	}
	
	// Este método prepara un nuevo producto. Se ejecuta cuando se instancia el constructor y cuando se entrega un producto terminado para
	// que siempre haya un producto nuevo listo para ser configurado.
	private void Reiniciar()
	{
		Console.WriteLine("Creando nueva transferencia...");
		this.transferencia = new Transferencia();
	}
}

// Constructor concreto para crear objetos que procesan las transferencias. Sirve para ilustrar:
// 1. Que los productos no tienen que tener ninguna interfaz común
// 2. Que es posible crear objetos muy diferentes pero compatibles, sólo se requiere que sus constructores implementen la misma interfaz
public class ConstructorProcesadorTransferencia : IConstructor<ProcesadorTransferencia>
{
	private ProcesadorTransferencia procesador;
	
	public ConstructorProcesadorTransferencia() => this.Reiniciar();
	
	public void FijarDatosBasicos(string cuentaDebito, string cuentaCredito, decimal monto)
	{
		Console.WriteLine($"Creando procesador para transferencia local...");
		this.procesador = new ProcesadorTransferenciaLocal();
	}
	
	public void FijarComentario(string descripcion)
	{
		Console.WriteLine("El comentario de la transferencia no altera el tipo de procesador.");
	}
	
	public void FijarBancoDestino(Banco banco)
	{
		if (Banco.Local != banco)
		{
			Console.WriteLine("Creando procesador para transferencia ACH...");
			this.procesador = new ProcesadorTransferenciaAch();
		}
	}
	
	public void FijarMonedaDestino(Moneda moneda)
	{
		if (Moneda.Local != moneda)
		{
			Console.WriteLine("Agregando conversor para moneda de destino al procesador...");
			this.procesador.ConversorMoneda = new ConversorMoneda(moneda);
		}
	}
	
	public void FijarAutorizacionesRequeridas(byte autorizaciones)
	{
		if (autorizaciones > 0)
		{
			Console.WriteLine("Creando procesador para transferencias con autorizaciones...");
			this.procesador = new ProcesadorTransferenciaConAutorizaciones();
		}
	}
	
	public ProcesadorTransferencia ObtenerResultado()
	{
		ProcesadorTransferencia procesador = this.procesador;
		Console.WriteLine("Entregando procesador de transferencia terminado...");
		this.Reiniciar();
		return procesador;
	}
	
	private void Reiniciar()
	{
		Console.WriteLine("Creando procesador de transferencia nulo...");
		this.procesador = new ProcesadorTransferencia();
	}
}

// La transferencia es uno de los productos, puede representar muchas combinaciones distintas de características. Por ejemplo:
// transferencias locales, ACH (interbancarias), con moneda cruzada, con autorizaciones, etc.
public class Transferencia
{
	public Transferencia()
	{
		this.BancoDestino = Banco.Local;
		this.MonedaDestino = Moneda.Local;
		this.AutorizacionesRequeridas = 0;
	}
	
	public string CuentaDebito { get; private set; }
	
	public string CuentaCredito { get; private set; }
	
	public decimal Monto { get; private set; }
	
	public string Comentario { get; set; }
	
	public Banco BancoDestino { get; set; }
	
	public Moneda MonedaDestino { get; set; }
	
	public byte AutorizacionesRequeridas { get; set; }
	
	public decimal MontoDestino { get; set; }
	
	public bool EsAch => Banco.Local != this.BancoDestino;
	
	public bool EsMonedaCruzada => Moneda.Local != this.MonedaDestino;
	
	public void FijarDatosBasicos(string cuentaDebito, string cuentaCredito, decimal monto)
	{
		this.CuentaDebito = cuentaDebito;
		this.CuentaCredito = cuentaCredito;
		this.Monto = monto;
	}
}

// El procesador es otro producto de un constructor que debe ser compatible con la transferencia. No se puede ejecutar una transferencia
// local con un procesador para transferencias ACH de moneda cruzada con autorizaciones.
public class ProcesadorTransferencia
{
	public ConversorMoneda ConversorMoneda { protected get; set; }

	public virtual bool Procesar(Transferencia transferencia)
	{
		Console.WriteLine("Procesador nulo, la transferencia no se ejecutará.");
		transferencia.Dump();
		return false;
	}
	
	protected void ConvertirMoneda(Transferencia transferencia)
	{
		if (transferencia.EsMonedaCruzada && this.ConversorMoneda != null)
		{
			transferencia.MontoDestino = this.ConversorMoneda.Convertir(transferencia.Monto);
		}
	}
}

// Los procesadores están implementados como subclases diferentes en lugar de ser construidos paso a paso. Esto se puede implementar de una
// mejor manera si se combina con otros patrones de diseño.
public class ProcesadorTransferenciaLocal : ProcesadorTransferencia
{
	public override bool Procesar(Transferencia transferencia)
	{
		if (transferencia.EsAch)
		{
			return false;
		}
		
		this.ConvertirMoneda(transferencia);
		
		Console.WriteLine($"Procesando transferencia local...");
		transferencia.Dump();
		return true;
	}
}

public class ProcesadorTransferenciaAch : ProcesadorTransferencia
{
	public override bool Procesar(Transferencia transferencia)
	{
		if (!transferencia.EsAch)
		{
			return false;
		}
		
		this.ConvertirMoneda(transferencia);

		Console.WriteLine($"Procesando transferencia ACH con banco de destino {transferencia.BancoDestino}...");
		transferencia.Dump();
		return true;
	}
}

public class ProcesadorTransferenciaConAutorizaciones : ProcesadorTransferencia
{
	public override bool Procesar(Transferencia transferencia)
	{
		if (transferencia.AutorizacionesRequeridas == 0)
		{
			return false;
		}
		
		Console.WriteLine($"La transferencia no se procesará, requiere {transferencia.AutorizacionesRequeridas} autorizaciones...");
		transferencia.Dump();
		return true;
	}
}

public enum Banco
{
	Local,
	BancoA,
	BancoB,
	BancoC,
}

public enum Moneda
{
	Local,
	MonedaA,
	MonedaB,
	MonedaC,
}

public class ConversorMoneda
{
	private Moneda monedaDestino;
	
	public ConversorMoneda(Moneda monedaDestino) => this.monedaDestino = monedaDestino;
	
	public decimal Convertir(decimal monto)
	{
		Console.WriteLine($"Convirtiendo monto a {this.monedaDestino}...");
		return monto * 5M;
	}
}