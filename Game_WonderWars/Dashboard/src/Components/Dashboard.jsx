import React from 'react';
import { Card, CardHeader, CardContent } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { LineChart, Line, XAxis, YAxis, Tooltip, ResponsiveContainer } from 'recharts';

const sampleData = [
  { date: '2025-06-01', completions: 12 },
  { date: '2025-06-02', completions: 18 },
  { date: '2025-06-03', completions: 10 },
  { date: '2025-06-04', completions: 22 },
  { date: '2025-06-05', completions: 16 },
];

export default function Dashboard() {
  return (
    <div className="p-6 space-y-6">
      <header className="flex items-center justify-between">
        <h1 className="text-2xl font-bold">VR Trivia Project Dashboard</h1>
        <Button>New Question</Button>
      </header>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <Card>
          <CardHeader>Monuments Imported</CardHeader>
          <CardContent className="text-3xl font-semibold">15</CardContent>
        </Card>
        <Card>
          <CardHeader>Total Questions</CardHeader>
          <CardContent className="text-3xl font-semibold">120</CardContent>
        </Card>
        <Card>
          <CardHeader>Daily Quiz Completions</CardHeader>
          <CardContent className="text-3xl font-semibold">22</CardContent>
        </Card>
      </div>

      <section className="space-y-4">
        <h2 className="text-xl font-medium">Activity Over Time</h2>
        <div className="h-64">
          <ResponsiveContainer width="100%" height="100%">
            <LineChart data={sampleData}>
              <XAxis dataKey="date" />
              <YAxis />
              <Tooltip />
              <Line type="monotone" dataKey="completions" stroke="#4F46E5" strokeWidth={2} />
            </LineChart>
          </ResponsiveContainer>
        </div>
      </section>

      <section className="space-y-4">
        <div className="flex items-center justify-between">
          <h2 className="text-xl font-medium">Question Bank</h2>
          <Input placeholder="Search questions..." />
        </div>
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>ID</TableHead>
              <TableHead>Monument</TableHead>
              <TableHead>Question</TableHead>
              <TableHead>Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            <TableRow>
              <TableCell>#1</TableCell>
              <TableCell>Colosseum</TableCell>
              <TableCell>Which city hosts the Colosseum?</TableCell>
              <TableCell>
                <Button size="sm" variant="outline" className="mr-2">Edit</Button>
                <Button size="sm" variant="destructive">Delete</Button>
              </TableCell>
            </TableRow>
            {/* More rows... */}
          </TableBody>
        </Table>
      </section>
    </div>
  );
}