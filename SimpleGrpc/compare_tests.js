import grpc from 'k6/net/grpc'; // DO NOT USE HTTPS SSL/TLS protocol conflicts appears
import http from 'k6/http';
import { check, sleep } from 'k6';

const client = new grpc.Client();
client.load(['./SharedContracts/Protos'], 'calculator.proto');

export const options = {
    scenarios: {
        grpc_scenario: {
            executor: 'constant-vus',
            vus: 50,
            duration: '30s',
            exec: 'runGrpc',
        },
        http_scenario: {
            executor: 'constant-vus',
            vus: 50,
            duration: '30s',
            exec: 'runHttp',
        },
    },
};

export function runGrpc() {
    client.connect('localhost:5120', { plaintext: true });

    const payload = { number_a: 10, number_b: 20 };
    const response = client.invoke('calculator.Calculator/Add', payload);
    
    check(response, {
        'gRPC Status OK': (r) => r && r.status === grpc.status_OK,
        'gRPC Correct Result': (r) => r.message && r.message.result === 30,
    });

    client.close();
    sleep(0.01);
}

export function runHttp() {
    const payload = JSON.stringify({ numberA: 10, numberB: 20 });
    const params = { headers: { 'Content-Type': 'application/json' } };

    const response = http.post('http://localhost:5058/api/calculator/add', payload, params);

    check(response, {
        'HTTP Status 200': (r) => r.status === 200,
        'HTTP Correct Result': (r) => {
            const body = JSON.parse(r.body);
            return body && body.result === 30;
        },
    });

    sleep(0.01);
}
