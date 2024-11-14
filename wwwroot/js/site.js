try {
    const permissionStatus = await navigator.permissions.query({ name: 'geolocation' });
    console.log(permissionStatus.state);
} catch (error) {
    if (error instanceof TypeError) {
        console.warn("Erro ignorado: ", error.message);
    } else {
        throw error; // Lança outros erros que não são TypeError
    }
}

try {
    // Coloque aqui o código que está gerando o erro
    // Por exemplo, se estiver usando uma função específica:
    Index();
} catch (error) {
    if (error instanceof TypeError) {
        console.warn("Erro TypeError ignorado: ", error.message);
    } else {
        console.error("Erro inesperado: ", error);
        throw error; // Opcional: relança outros erros, se necessário
    }
}
