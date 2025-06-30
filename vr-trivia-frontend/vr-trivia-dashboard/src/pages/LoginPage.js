// src/pages/LoginPage.js
import { useState, useContext } from 'react';
import {
  Box,
  Button,
  Heading,
  Text,
  VStack,
  Alert, // Use AlertIcon for Chakra UI v2/v1
} from '@chakra-ui/react';
import { AuthContext } from '../context/AuthContext';

export default function LoginPage() {
  const { login } = useContext(AuthContext);
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      await login({ email, password }); // From AuthContext
    } catch (err) {
      setError(err.response?.data?.message || 'Login failed.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box maxW="md" mx="auto" mt="100px" p="8" borderWidth="1px" borderRadius="lg" boxShadow="lg">
      <VStack spacing="6">
        <Heading size="lg">Login</Heading>

        {error && (
          <Alert status="error" borderRadius="md">
            <Text>{error}</Text>
          </Alert>
        )}

        <form onSubmit={handleSubmit} style={{ width: '100%' }}>
          <div style={{ marginBottom: '1rem' }}>
            <label htmlFor="email">Email</label>
            <input
              id="email"
              type="email"
              placeholder="Enter your email"
              value={email}
              required
              style={{ width: '100%', padding: '8px', marginTop: '4px' }}
              onChange={(e) => setEmail(e.target.value)}
            />
          </div>

          <div style={{ marginBottom: '1.5rem' }}>
            <label htmlFor="password">Password</label>
            <input
              id="password"
              type="password"
              placeholder="Enter your password"
              value={password}
              required
              style={{ width: '100%', padding: '8px', marginTop: '4px' }}
              onChange={(e) => setPassword(e.target.value)}
            />
          </div>

          <Button type="submit" colorScheme="teal" width="full" isLoading={loading}>
            Login
          </Button>
        </form>

        <Text fontSize="sm" color="gray.500">
          Use your registered credentials to access the dashboard.
        </Text>
      </VStack>
    </Box>
  );
}